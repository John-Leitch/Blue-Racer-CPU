using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidPreprocessor
    {
        private IdentifierExpression gotoId = new IdentifierExpression("goto"),
                gotoFalseId = new IdentifierExpression("gotoFalse");

        private AphidMacro GetMacro(CallExpression call, AphidMacro[] macros)
        {
            var idExp = call.FunctionExpression as IdentifierExpression;

            if (idExp == null)
            {
                return null;
            }

            return macros.SingleOrDefault(x => x.Name == idExp.Identifier);
        }

        private AphidMacro[] macros = new AphidMacro[0];

        public List<Expression> ExpandMacros(List<Expression> ast)
        {
            var ast2 = new List<Expression>(ast);
            macros = macros.Concat(AphidMacro.Parse(ast)).ToArray();
            var removed = ast2.RemoveAll(x => macros.Any(y => y.OriginalExpression == x));

            return ExpandMacros(ast2, macros);
        }

        private List<Expression> ExpandMacros(List<Expression> ast, AphidMacro[] macros)
        {
            if (ast == null)
            {
                return null;
            }

            var ast2 = new List<Expression>();

            foreach (var exp in ast)
            {
                ast2.AddRange(ExpandMacros(exp, macros));
            }

            return ast2;
        }

        private List<Expression> ExpandMacros(Expression expression, AphidMacro[] macros)
        {
            //var cfg = ExpandControlFlowExpressions(expression);

            //if (cfg != null)
            //{
            //    return cfg.SelectMany(x => ExpandMacros(x, macros)).ToList();
            //}

            var expanded = new List<Expression>();

            if (expression is CallExpression)
            {
                var call = (CallExpression)expression;
                var macro = GetMacro((CallExpression)expression, macros);

                if (macro != null)
                {
                    expanded.AddRange(macro.Replace(call.Args));
                }
                else
                {
                    expanded.Add(new CallExpression(
                        ExpandMacros(call.FunctionExpression, macros).Single(),
                        call.Args.Select(x => ExpandMacros(x, macros).Single())));
                }
            }
            else if (expression is UnaryOperatorExpression)
            {
                var unOp = (UnaryOperatorExpression)expression;

                expanded.Add(new UnaryOperatorExpression(
                    unOp.Operator,
                    ExpandMacros(unOp.Operand, macros).Single())
                {
                    IsPostfix = unOp.IsPostfix
                });
            }
            else if (expression is BinaryOperatorExpression)
            {
                var binOp = (BinaryOperatorExpression)expression;

                expanded.Add(new BinaryOperatorExpression(
                    ExpandMacros(binOp.LeftOperand, macros).Single(),
                    binOp.Operator,
                    ExpandMacros(binOp.RightOperand, macros).Single()));
            }
            else if (expression is IfExpression)
            {
                var ifExp = (IfExpression)expression;

                expanded.Add(new IfExpression(
                    ExpandMacros(ifExp.Condition, macros).Single(),
                    ExpandMacros(ifExp.Body, macros),
                    ExpandMacros(ifExp.ElseBody, macros)));
            }
            else if (expression is ControlFlowExpression)
            {
                var cfExp = (ControlFlowExpression)expression;

                expanded.Add(new ControlFlowExpression(
                    cfExp.Type,
                    ExpandMacros(cfExp.Condition, macros).Single(),
                    ExpandMacros(cfExp.Body, macros)));

            }
            else if (expression is ForExpression)
            {
                var forExp = (ForExpression)expression;

                expanded.Add(new ForExpression(
                    ExpandMacros(forExp.Initialization, macros).Single(),
                    ExpandMacros(forExp.Condition, macros).Single(),
                    ExpandMacros(forExp.Afterthought, macros).Single(),
                    ExpandMacros(forExp.Body, macros)));
            }
            else if (expression is LoadScriptExpression)
            {
                var lsExp = (LoadScriptExpression)expression;

                expanded.Add(new LoadScriptExpression(
                    ExpandMacros(lsExp.FileExpression, macros).Single()));
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

        public List<Expression> ExpandIfExpression(IfExpression expression)
        {
            var g = Guid.NewGuid();

            IdentifierExpression ifLabel = new IdentifierExpression("If_" + g),
                elseLabel = new IdentifierExpression("Else_" + g),
                endIfLabel = new IdentifierExpression("EndIf_" + g);

            var ast = new List<Expression>
            {
                ifLabel,
                expression.Condition,
                new CallExpression(gotoFalseId, elseLabel)
            };

            ast.AddRange(expression.Body);
            ast.Add(new CallExpression(gotoId, endIfLabel));
            ast.Add(elseLabel);
            ast.AddRange(expression.ElseBody);
            ast.Add(endIfLabel);

            return ast;
        }

        public List<Expression> ExpandWhileExpression(ControlFlowExpression expression)
        {
            var g = Guid.NewGuid();

            IdentifierExpression whileLabel = new IdentifierExpression("While_" + g),
                endWhileLabel = new IdentifierExpression("EndWhileIf_" + g);

            var ast = new List<Expression>
            {
                whileLabel,
                expression.Condition,
                new CallExpression(gotoFalseId, endWhileLabel),
            };

            ast.AddRange(expression.Body);
            ast.Add(new CallExpression(gotoId, whileLabel));
            ast.Add(endWhileLabel);

            return ast;
        }

        private bool AnyControlFlowExpressions(List<Expression> ast)
        {
            return ast.OfType<IfExpression>().Any() || ast.OfType<ControlFlowExpression>().Any();
        }

        private List<Expression> ExpandControlFlowExpressions(Expression expression)
        {
            if (expression is IfExpression)
            {
                return ExpandIfExpression((IfExpression)expression);
            }
            else if (expression is ControlFlowExpression)
            {
                var cfExp = (ControlFlowExpression)expression;

                switch (cfExp.Type)
                {
                    case AphidTokenType.whileKeyword:
                        return ExpandWhileExpression(cfExp);

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return null;
            }
        }

        public List<Expression> ExpandControlFlowExpressions(List<Expression> ast)
        {
            var ast2 = new List<Expression>(ast);

            var ifs = ast
                .Select(x => new
                {
                    Expression = x,
                    Expanded = ExpandControlFlowExpressions(x),
                })
                .Where(x => x.Expanded != null)
                .ToArray();

            foreach (var expandedIf in ifs)
            {
                var i = ast2.IndexOf(expandedIf.Expression);
                ast2.RemoveAt(i);
                ast2.InsertRange(i, expandedIf.Expanded);
            }

            if (AnyControlFlowExpressions(ast2))
            {
                return ExpandControlFlowExpressions(ast2);
            }
            else
            {
                //foreach (var n in ast.OfType<IParentNode>())
                //{
                //    ExpandControlFlowExpressions(n.GetChildren().ToList());
                //}

                return ast2;
            }
        }
    }
}
