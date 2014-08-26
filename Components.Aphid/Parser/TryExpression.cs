using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class TryExpression : Expression, IParentNode
    {
        public List<Expression> TryBody { get; set; }

        public List<Expression> CatchBody { get; set; }

        public IdentifierExpression CatchArg { get; set; }

        public List<Expression> FinallyBody { get; set; }

        public IEnumerable<Expression> GetChildren()
        {
            return TryBody.Concat(CatchBody).Concat(new[] { CatchArg }).Concat(FinallyBody);
        }
    }
}
