using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class LoadLibraryExpression : Expression, IParentNode
    {
        public Expression LibraryExpression { get; set; }

        public LoadLibraryExpression(Expression libraryExpression)
        {
            LibraryExpression = libraryExpression;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { LibraryExpression };
        }
    }
}
