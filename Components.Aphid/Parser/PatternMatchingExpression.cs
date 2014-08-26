using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class PatternMatchingExpression : Expression, IParentNode
    {
        public Expression TestExpression { get; set; }

        public List<Tuple<Expression, Expression>> Patterns { get; set; }

        public PatternMatchingExpression()
        {
            Patterns = new List<Tuple<Expression, Expression>>();
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { TestExpression }.Concat(Patterns.SelectMany(x => new[] { x.Item1, x.Item2 }));
        }
    }
}
