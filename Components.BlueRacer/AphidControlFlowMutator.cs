using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Components.BlueRacer
{
    public class AphidControlFlowMutator : AphidMutator
    {
        private IdentifierExpression gotoId = new IdentifierExpression("goto"),
                gotoFalseId = new IdentifierExpression("gotoFalse");

        private AphidExpression MutateCondition(AphidExpression condition)
        {
            if (condition is IdentifierExpression)
            {
                return new BinaryOperatorExpression(
                    condition,
                    AphidTokenType.NotEqualOperator,
                    new NumberExpression(0));
            }
            else
            {
                return condition;
            }
        }

        public List<AphidExpression> ExpandIfExpression(IfExpression expression)
        {
            var g = Guid.NewGuid();

            IdentifierExpression ifLabel = new IdentifierExpression("If_" + g),
                elseLabel = new IdentifierExpression("Else_" + g),
                endIfLabel = new IdentifierExpression("EndIf_" + g);

            var ast = new List<AphidExpression>
            {
                ifLabel,
                MutateCondition(expression.Condition),
                new CallExpression(gotoFalseId, elseLabel)
            };

            ast.AddRange(expression.Body);
            ast.Add(new CallExpression(gotoId, endIfLabel));
            ast.Add(elseLabel);

            if (expression.ElseBody != null)
            {
                ast.AddRange(expression.ElseBody);
            }

            ast.Add(endIfLabel);

            return ast;
        }

        public List<AphidExpression> ExpandWhileExpression(ControlFlowExpression expression)
        {
            var g = Guid.NewGuid();

            IdentifierExpression whileLabel = new IdentifierExpression("While_" + g),
                endWhileLabel = new IdentifierExpression("EndWhileIf_" + g);

            var ast = new List<AphidExpression>
            {
                whileLabel,
                MutateCondition(expression.Condition),
                new CallExpression(gotoFalseId, endWhileLabel),
            };

            ast.AddRange(expression.Body);
            ast.Add(new CallExpression(gotoId, whileLabel));
            ast.Add(endWhileLabel);

            return ast;
        }

        public List<AphidExpression> ExpandForExpression(ForExpression expression)
        {
            var body = new List<AphidExpression>(expression.Body);
            body.Add(expression.Afterthought);

            return new List<AphidExpression>()
            {
                expression.Initialization,
                new WhileExpression(MutateCondition(expression.Condition), body),
            };
        }

        private bool AnyControlFlowExpressions(List<AphidExpression> ast)
        {
            return ast.OfType<IfExpression>().Any() || ast.OfType<ControlFlowExpression>().Any();
        }

        private List<AphidExpression> ExpandControlFlowExpressions(AphidExpression expression)
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
                    case AphidNodeType.WhileExpression:
                        return ExpandWhileExpression(cfExp);

                    default:
                        throw new NotImplementedException();
                }
            }
            else if (expression is ForExpression)
            {
                return ExpandForExpression((ForExpression)expression);
            }
            else
            {
                return null;
            }
        }

        protected override List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged)
        {
            var ast = ExpandControlFlowExpressions(expression);
            hasChanged = ast != null;
            return ast;
        }
    }
}
