using System;
using System.Linq;
using System.Collections.Generic;

namespace Components.Aphid.Parser
{
    public class ForEachExpression : Expression, IParentNode
    {
        public Expression Collection { get; set; }

        public Expression Element { get; set; }

        public List<Expression> Body { get; set; }

        public ForEachExpression (Expression collection, Expression element, List<Expression> body)
        {
            Collection = collection;
            Element = element;
            Body = body;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { Collection, Element }.Concat(Body);
        }
    }
}

