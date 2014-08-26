using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class LoadScriptExpression : Expression, IParentNode
    {
        public Expression FileExpression { get; set; }

        public LoadScriptExpression(Expression fileExpression)
        {
            FileExpression = fileExpression;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { FileExpression };
        }
    }
}
