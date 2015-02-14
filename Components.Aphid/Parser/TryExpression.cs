using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class TryExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.TryExpression; }
        }

        public List<AphidExpression> TryBody { get; set; }

        public List<AphidExpression> CatchBody { get; set; }

        public IdentifierExpression CatchArg { get; set; }

        public List<AphidExpression> FinallyBody { get; set; }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return TryBody.Concat(CatchBody).Concat(new[] { CatchArg }).Concat(FinallyBody);
        }
    }
}
