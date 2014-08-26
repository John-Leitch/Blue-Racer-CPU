using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class SwitchCase : IParentNode
    {
        public List<Expression> Cases { get; set; }

        public List<Expression> Body { get; set; }

        public IEnumerable<Expression> GetChildren()
        {
            return Cases.Concat(Body);
        }
    }
}
