using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Aphid.Parser
{
    public class ControlFlowExpression : Expression, IParentNode
    {
        public AphidTokenType Type { get; set; }

        public Expression Condition { get; set; }

        public List<Expression> Body { get; set; }

        public ControlFlowExpression(AphidTokenType type, Expression condition, List<Expression> body)
        {
            Type = type;
            Condition = condition;
            Body = body;
        }

        public virtual IEnumerable<Expression> GetChildren()
        {
            return new[] { Condition }.Concat(Body);
        }
    }
}
