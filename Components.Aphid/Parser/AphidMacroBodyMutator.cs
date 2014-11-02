using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class AphidMacroBodyMutator : AphidMutator
    {
        private Dictionary<string, Expression> _arguments;

        public AphidMacroBodyMutator(Dictionary<string, Expression> arguments)
        {
            _arguments = arguments;
        }

        protected override List<Expression> MutateCore(Expression expression, out bool hasChanged)
        {
            var idExp = expression as IdentifierExpression;

            Expression argument;

            if (idExp == null || !_arguments.TryGetValue(idExp.Identifier, out argument))
            {
                hasChanged = false;

                return null;
            }

            hasChanged = true;

            return new List<Expression> { argument };
        }
    }
}
