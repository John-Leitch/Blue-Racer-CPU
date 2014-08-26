using Components.Aphid.Interpreter;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidIncludeMutator : AphidMutator
    {
        protected override List<Expression> MutateCore(Expression expression, out bool hasChanged)
        {
            if (!(expression is LoadScriptExpression))
            {
                hasChanged = false;

                return null;
            }

            hasChanged = true;            

            var str = ((StringExpression)((LoadScriptExpression)expression).FileExpression).Value;
            str = StringParser.Parse(str);

            var strFile = new AphidLoader(null).FindScriptFile(str);

            return AphidParser.Parse(File.ReadAllText(strFile));
        }
    }
}
