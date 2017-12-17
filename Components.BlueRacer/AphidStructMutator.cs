using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Components.BlueRacer
{
    public class AphidStructMutator : AphidMutator
    {
        private StructResolver _typeResolver = new StructResolver();

        private Dictionary<string, string> _registerTypeTable = new Dictionary<string, string>();

        public override void Reset()
        {
            _registerTypeTable.Clear();
            base.Reset();
        }

        private bool IsRegister(string identifier)
        {
            return Regex.IsMatch(identifier, @"^r\d+$");
        }

        private string[] FlattenMembers(AphidExpression memberExpression)
        {
            switch (memberExpression.Type)
            {
                case AphidExpressionType.IdentifierExpression:
                    return new[] { ((IdentifierExpression)memberExpression).Identifier };

                case AphidExpressionType.BinaryOperatorExpression:
                    var binOpExp = (BinaryOperatorExpression)memberExpression;

                    return FlattenMembers(binOpExp.LeftOperand)
                        .Concat(FlattenMembers(binOpExp.RightOperand))
                        .ToArray();

                default:
                    throw new InvalidOperationException();
            }
        }

        private List<AphidExpression> MutateMemberExpression(BinaryOperatorExpression memberExpression)
        {
            var path = FlattenMembers(memberExpression);
            var register = path.First();

            if (!IsRegister(register))
            {
                throw new InvalidOperationException();
            }

            var registerType = _registerTypeTable[register];
            var offset = _typeResolver.ResolveOffset(registerType, path.Skip(1).ToArray());

            var exp = new UnaryOperatorExpression(
                AphidTokenType.MultiplicationOperator,
                new BinaryOperatorExpression(
                    new IdentifierExpression(register),
                    AphidTokenType.AdditionOperator,
                    new NumberExpression(offset)),
                isPostfix: false);

            return new List<AphidExpression> { exp };
        }

        protected override List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged)
        {
            hasChanged = false;

            switch (expression.Type)
            {
                case AphidExpressionType.BinaryOperatorExpression:
                    var binOpExp = (BinaryOperatorExpression)expression;

                    if (binOpExp.Operator == AphidTokenType.AssignmentOperator &&
                        binOpExp.LeftOperand.Type == AphidExpressionType.IdentifierExpression &&
                        binOpExp.RightOperand.Type == AphidExpressionType.ObjectExpression)
                    {
                        hasChanged = true;

                        _typeResolver.InterpretType(
                            (IdentifierExpression)binOpExp.LeftOperand,
                            (ObjectExpression)binOpExp.RightOperand);

                        return new List<AphidExpression>();
                    }
                    else if (binOpExp.Operator == AphidTokenType.MemberOperator)
                    {
                        hasChanged = true;

                        return MutateMemberExpression(binOpExp);
                    }

                    break;

                case AphidExpressionType.IdentifierExpression:

                    var id = (IdentifierExpression)expression;

                    if (IsRegister(id.Identifier) && id.Attributes.Any())
                    {
                        var type = _typeResolver.GetType(id);
                        _registerTypeTable.Add(id.Identifier, type);
                    }

                    break;
            }

            return null;
        }
    }
}
