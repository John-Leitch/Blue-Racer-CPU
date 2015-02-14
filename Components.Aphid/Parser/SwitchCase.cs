using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class SwitchCase : IParentNode
    {
        public List<AphidExpression> Cases { get; set; }

        public List<AphidExpression> Body { get; set; }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return Cases.Concat(Body);
        }
    }
}
