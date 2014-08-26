using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidMacro
    {
        public string Name { get; private set; }

        public FunctionExpression Declaration { get; private set; }

        public BinaryOperatorExpression OriginalExpression { get; private set; }

        public AphidMacro(string name, FunctionExpression declaration, BinaryOperatorExpression originalExpression)
        {
            Name = name;
            Declaration = declaration;
            OriginalExpression = originalExpression;
        }

        public static AphidMacro[] Parse(List<Expression> ast)
        {
            return ast
                .OfType<BinaryOperatorExpression>()
                .Select(x => new
                {
                    Id = x.LeftOperand as IdentifierExpression,
                    Call = x.RightOperand as CallExpression,
                    Original = x,
                })
                .Where(x => x.Id != null && x.Call != null && x.Call.Args.Count() == 1)
                .Select(x => new
                {
                    Name = x.Id.Identifier,
                    CallName = x.Call.FunctionExpression as IdentifierExpression,
                    Func = x.Call.Args.Single() as FunctionExpression,
                    x.Original,
                })
                .Where(x =>
                    x.CallName != null &&
                    x.CallName.Identifier == InstructionMnemonic.Macro &&
                    x.Func != null)
                .Select(x => new AphidMacro(x.Name, x.Func, x.Original))
                .ToArray();
        }

        public List<Expression> Replace(IEnumerable<Expression> values)
        {
            if (values.Count() != Declaration.Args.Count())
            {
                throw new InvalidOperationException();
            }

            var kvps = values
                .Select((x, i) =>
                    new KeyValuePair<string, Expression>(Declaration.Args[i].Identifier, x))
                .ToDictionary(x => x.Key, x => x.Value);

            var replaced = new List<Expression>();

            foreach (var exp in Declaration.Body)
            {
                replaced.Add(Replace(kvps, exp));
            }

            return replaced;
        }

        private Expression Replace(Dictionary<string, Expression> values, Expression expression)
        {
            if (expression is BinaryOperatorExpression)
            {
                var binOp = (BinaryOperatorExpression)expression;
                
                return new BinaryOperatorExpression(
                    Replace(values, binOp.LeftOperand), 
                    binOp.Operator, 
                    Replace(values, binOp.RightOperand));
            }
            else if (expression is UnaryOperatorExpression)
            {
                var unOp = (UnaryOperatorExpression)expression;

                return new UnaryOperatorExpression(
                    unOp.Operator, 
                    Replace(values, unOp.Operand)) 
                    { 
                        IsPostfix = unOp.IsPostfix 
                    };
            }
            else if (expression is CallExpression)
            {
                var call = (CallExpression)expression;

                return new CallExpression(
                    Replace(values, call.FunctionExpression),
                    call.Args.Select(x => Replace(values, x)));
            }
            else if (expression is IdentifierExpression)
            {
                var id = ((IdentifierExpression)expression).Identifier;

                if (values.ContainsKey(id))
                {
                    return values[id];
                }
                else
                {
                    return expression;
                }
            }
            else if (expression is IfExpression)
            {
                var ifExp = (IfExpression)expression;

                return new IfExpression(
                    Replace(values, ifExp.Condition),
                    ifExp.Body.Select(x => Replace(values, x)).ToList(),
                    ifExp.ElseBody != null ? 
                        ifExp.ElseBody.Select(x => Replace(values, x)).ToList() :
                        null);
            }
            else if (expression is ControlFlowExpression)
            {
                var cfExp = (ControlFlowExpression)expression;

                return new ControlFlowExpression(
                    cfExp.Type,
                    Replace(values, cfExp.Condition),
                    cfExp.Body.Select(x => Replace(values, x)).ToList());
            }
            else if (expression is IParentNode)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return expression;
            }
        }
    }
}
