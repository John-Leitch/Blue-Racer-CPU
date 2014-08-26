using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class SwitchExpression : Expression, IParentNode
    {
        public Expression Expression { get; set; }

        public List<SwitchCase> Cases { get; set; }

        public List<Expression> DefaultCase { get; set; }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { Expression }
                .Concat(Cases.SelectMany(x => x.GetChildren()))
                .Concat(DefaultCase);
        }
    }
}
