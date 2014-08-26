using System;
using System.Collections.Generic;

namespace Components.Aphid.Parser
{
    public class DynamicMemberExpression : Expression, IParentNode
    {
        public Expression MemberExpression { get; set; }

        public DynamicMemberExpression ()
        {
        }

        public DynamicMemberExpression (Expression memberExpression)
        {
            MemberExpression = memberExpression;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { MemberExpression };
        }
    }
}

