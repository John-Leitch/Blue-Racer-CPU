using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidCallMutator : AphidMutator
    {
        private string[] _mnemonics = InstructionMnemonic.GetAll();

        private CallExpression CreatePush(AphidExpression value)
        {
            return new CallExpression(
                new IdentifierExpression(InstructionMnemonic.Push),
                value);
        }

        protected override List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged)
        {
            var call = expression as CallExpression;
            string funcName = null;

            if (call != null)
            {
                var id = call.FunctionExpression as IdentifierExpression;

                if (id != null)
                {
                    funcName = id.Identifier;
                }
            }

            if (funcName == null || _mnemonics.Contains(funcName))
            {
                hasChanged = false;

                return null;
            }

            var mutated = new List<AphidExpression>();
            mutated.AddRange(call.Args.Reverse().Select(CreatePush));
            mutated.Add(new CallExpression(
                new IdentifierExpression(InstructionMnemonic.Call),
                call.FunctionExpression));

            var argSize = call.Args.Count() * 4;

            if (argSize != 0)
            {
                mutated.Add(new BinaryOperatorExpression(
                    new IdentifierExpression("r0"),
                    AphidTokenType.PlusEqualOperator,
                    new NumberExpression(argSize)));
            }

            hasChanged = true;

            return mutated;
        }
    }
}
