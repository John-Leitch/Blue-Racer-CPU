using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class ExtendExpression : Expression, IParentNode
    {
        public string Type { get; set; }

        public ObjectExpression Object { get; set; }

        public ExtendExpression(string type, ObjectExpression obj)
        {
            Type = type;
            Object = obj;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { Object };
        }
    }
}
