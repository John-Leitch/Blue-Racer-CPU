using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public abstract class AphidMutator
    {
        public bool HasMutated { get; private set; }

        protected abstract List<Expression> MutateCore(Expression expression, out bool hasChanged);

        public List<Expression> Mutate(List<Expression> ast)
        {
            if (ast == null)
            {
                return null;
            }

            var ast2 = new List<Expression>();

            foreach (var exp in ast)
            {
                ast2.AddRange(Mutate(exp));
            }

            return ast2;
        }

        public List<Expression> Mutate(Expression expression)
        {

            //var cfg = ExpandControlFlowExpressions(expression);

            //if (cfg != null)
            //{
            //    return cfg.SelectMany(x => ExpandMacros(x, macros)).ToList();
            //}
            bool hasChanged;
            var mutated = MutateCore(expression, out hasChanged);

            if (hasChanged)
            {
                HasMutated = true;
                return mutated.SelectMany(Mutate).ToList();
            }

            var expanded = new List<Expression>();

            if (expression is CallExpression)
            {
                var call = (CallExpression)expression;

                expanded.Add(new CallExpression(
                    Mutate(call.FunctionExpression).Single(),
                    call.Args.Select(x => Mutate(x).Single()).ToArray()));
            }
            else if (expression is UnaryOperatorExpression)
            {
                var unOp = (UnaryOperatorExpression)expression;

                expanded.Add(new UnaryOperatorExpression(
                    unOp.Operator,
                    Mutate(unOp.Operand).Single())
                {
                    IsPostfix = unOp.IsPostfix
                });
            }
            else if (expression is BinaryOperatorExpression)
            {
                var binOp = (BinaryOperatorExpression)expression;

                expanded.Add(new BinaryOperatorExpression(
                    Mutate(binOp.LeftOperand).Single(),
                    binOp.Operator,
                    Mutate(binOp.RightOperand).Single()));
            }
            else if (expression is IfExpression)
            {
                var ifExp = (IfExpression)expression;

                expanded.Add(new IfExpression(
                    Mutate(ifExp.Condition).Single(),
                    Mutate(ifExp.Body),
                    Mutate(ifExp.ElseBody)));
            }
            else if (expression is ForExpression)
            {
                var forExp = (ForExpression)expression;

                expanded.Add(new ForExpression(
                    Mutate(forExp.Initialization).Single(),
                    Mutate(forExp.Condition).Single(),
                    Mutate(forExp.Afterthought).Single(),
                    Mutate(forExp.Body)));
            }
            else if (expression is ControlFlowExpression)
            {
                var cfExp = (ControlFlowExpression)expression;

                expanded.Add(new ControlFlowExpression(
                    cfExp.Type,
                    Mutate(cfExp.Condition).Single(),
                    Mutate(cfExp.Body)));
            }
            else if (expression is LoadScriptExpression)
            {
                var lsExp = (LoadScriptExpression)expression;

                expanded.Add(new LoadScriptExpression(Mutate(lsExp.FileExpression).Single()));
            }
            else if (expression is FunctionExpression)
            {
                var funcExp = (FunctionExpression)expression;

                expanded.Add(new FunctionExpression()
                {
                    Args = funcExp.Args.Select(x => (IdentifierExpression)Mutate(x).Single()).ToList(),
                    Body = funcExp.Body.SelectMany(x => Mutate(x)).ToList()
                });
            }
            else if (expression is IParentNode)
            {
                throw new InvalidOperationException();
            }
            else
            {
                expanded.Add(expression);
            }

            return expanded;
        }

        public List<Expression> MutateRecursively(List<Expression> expression)
        {
            List<Expression> ast = expression;
            
            do
            {
                Reset();
                ast = Mutate(ast);
            } while (HasMutated);

            return ast;
        }

        public void Reset()
        {
            HasMutated = false;
        }
    }
}
