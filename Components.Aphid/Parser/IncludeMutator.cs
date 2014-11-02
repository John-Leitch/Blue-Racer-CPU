using Components.Aphid.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class IncludeMutator : AphidMutator
    {
        AphidLoader _loader = new AphidLoader(null);

        public AphidLoader Loader
        {
            get { return _loader; }
        }

        protected override List<Expression> MutateCore(Expression expression, out bool hasChanged)
        {
            var loadExp = expression as LoadScriptExpression;

            if (loadExp == null)
            {
                hasChanged = false;

                return null;
            }

            hasChanged = true;

            var scriptExp = loadExp.FileExpression as StringExpression;

            if (scriptExp == null)
            {
                throw new InvalidOperationException();
            }

            var script = _loader.FindScriptFile(StringParser.Parse(scriptExp.Value));

            if (!File.Exists(script))
            {
                throw new InvalidOperationException();
            }

            return AphidParser.Parse(File.ReadAllText(script));
        }
    }
}
