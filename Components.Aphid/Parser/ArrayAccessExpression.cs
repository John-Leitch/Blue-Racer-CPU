using System;
using System.Collections.Generic;

namespace Components.Aphid.Parser
{
    public class ArrayAccessExpression : Expression, IParentNode
    {
        public Expression ArrayExpression { get; set; }

        public Expression KeyExpression { get; set; }

        public ArrayAccessExpression ()
        {
        }

        public ArrayAccessExpression(Expression arrayExpression, Expression keyExpression)
        {
            ArrayExpression = arrayExpression;
            KeyExpression = keyExpression;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { ArrayExpression, KeyExpression };
        }
    }
}

