using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class ConstantFoldingMutator : AphidMutator
    {
        private bool OperandsAre<T>(BinaryOperatorExpression binOp)
        {
            return binOp.LeftOperand.GetType() == typeof(T) &&
                binOp.RightOperand.GetType() == typeof(T);
        }

        private decimal GetNumber(Expression exp)
        {
            return ((NumberExpression)exp).Value;
        }

        private string GetString(Expression exp)
        {
            return ((StringExpression)exp).Value;
        }

        protected override List<Expression> MutateCore(Expression expression, out bool hasChanged)
        {
            var binOp = expression as BinaryOperatorExpression;

            hasChanged = false;

            if (binOp == null)
            {
                return null;
            }

            hasChanged = true;

            if (OperandsAre<StringExpression>(binOp) &&
                binOp.Operator == AphidTokenType.AdditionOperator)
            {
                var left = GetString(binOp.LeftOperand);
                var right = GetString(binOp.RightOperand);

                if (left[0] != right[0])
                {
                    hasChanged = false;

                    return null;
                }

                return new List<Expression> 
                { 
                    new StringExpression(left.Remove(left.Length - 1) + right.Substring(1))
                };
            }
            else if (OperandsAre<NumberExpression>(binOp))
            {
                var left = GetNumber(binOp.LeftOperand);
                var right = GetNumber(binOp.RightOperand);

                switch (binOp.Operator)
                {
                    case AphidTokenType.AdditionOperator:
                        return new List<Expression> { new NumberExpression(left + right) };

                    case AphidTokenType.MinusOperator:
                        return new List<Expression> { new NumberExpression(left - right) };

                    default:
                        hasChanged = false;
                        break;
                }
            }
            else
            {
                hasChanged = false;
            }

            return null;
        }
    }
}
