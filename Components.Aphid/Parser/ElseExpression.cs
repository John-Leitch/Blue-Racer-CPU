using System;
using System.Collections.Generic;

namespace Components.Aphid.Parser
{
    public class ElseExpression : IParentNode
    {
        public List<Expression> Body { get; set; }

        public ElseExpression()
        {
        }

        public ElseExpression(List<Expression> body)
        {
            Body = body;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return Body;
        }
    }
}

