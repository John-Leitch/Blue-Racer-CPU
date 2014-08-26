using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class PartialFunctionExpression : Expression, IParentNode
    {
        public CallExpression Call { get; set; }

        public PartialFunctionExpression()
        {
        }

        public PartialFunctionExpression(CallExpression call)
        {
            Call = call;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { Call };
        }
    }
}
