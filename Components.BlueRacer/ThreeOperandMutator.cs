using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class ThreeOperandMutator : AphidMutator
    {
        private Dictionary<AphidTokenType, AphidTokenType> _opTable = new Dictionary<AphidTokenType, AphidTokenType>
        {
            { AphidTokenType.AdditionOperator, AphidTokenType.PlusEqualOperator },
            { AphidTokenType.MinusOperator, AphidTokenType.MinusEqualOperator },
            { AphidTokenType.MultiplicationOperator, AphidTokenType.MultiplicationEqualOperator },
            { AphidTokenType.DivisionOperator, AphidTokenType.DivisionEqualOperator },
            { AphidTokenType.ModulusOperator, AphidTokenType.ModulusEqualOperator },
            { AphidTokenType.ShiftLeft, AphidTokenType.ShiftLeftEqualOperator },
            { AphidTokenType.ShiftRight, AphidTokenType.ShiftRightEqualOperator },
            { AphidTokenType.XorEqualOperator, AphidTokenType.XorEqualOperator },
            { AphidTokenType.BinaryAndOperator, AphidTokenType.BinaryAndEqualOperator },
            { AphidTokenType.BinaryOrOperator, AphidTokenType.OrEqualOperator },
        };

        private AphidTokenType MutateOperator(AphidTokenType tokenType)
        {
            AphidTokenType mutatedType;


            if (!_opTable.TryGetValue(tokenType, out mutatedType))
            {
                throw new InvalidOperationException();
            }

            return mutatedType;
        }

        // Todo:
        // Handle Rx = Rx + y
        // Handle Rx = *Ry + z
        protected override List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged)
        {
            hasChanged = false;

            var binOpExp = expression as BinaryOperatorExpression;

            if (binOpExp == null)
            {
                return null;
            }

            var binOpOperand = binOpExp.RightOperand as BinaryOperatorExpression;

            if (binOpOperand == null)
            {
                return null;
            }

            hasChanged = true;
            AphidExpression left = binOpOperand.LeftOperand, right = binOpOperand.RightOperand;

            if (right is IdentifierExpression && !(left is IdentifierExpression))
            {
                var tmp = left;
                left = right;
                right = tmp;
            }

            var destinationExp = binOpExp.LeftOperand as IdentifierExpression;

            if (destinationExp == null)
            {
                throw new InvalidOperationException();
            }

            var operandLeftOperand = left as IdentifierExpression;

            if (operandLeftOperand.Identifier.StartsWith("in"))
            {
                var assignmentExp = new BinaryOperatorExpression(
                    destinationExp,
                    AphidTokenType.AssignmentOperator,
                    operandLeftOperand);

                var operationExp = new BinaryOperatorExpression(
                    destinationExp,
                    MutateOperator(binOpOperand.Operator),
                    right);

                return new List<AphidExpression> { assignmentExp, operationExp };
            }
            else
            {
                var operationExp = new BinaryOperatorExpression(
                    left,
                    MutateOperator(binOpOperand.Operator),
                    right);                

                var assignmentExp = new BinaryOperatorExpression(
                    destinationExp,
                    AphidTokenType.AssignmentOperator,
                    left);

                return new List<AphidExpression> { operationExp, assignmentExp };
            }
        }
    }
}
