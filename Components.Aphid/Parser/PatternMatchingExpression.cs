using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class PatternMatchingExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.PatternMatchingExpression; }
        }

        public AphidExpression TestExpression { get; set; }

        public List<Tuple<AphidExpression, AphidExpression>> Patterns { get; set; }

        public PatternMatchingExpression()
        {
            Patterns = new List<Tuple<AphidExpression, AphidExpression>>();
        }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return new[] { TestExpression }.Concat(Patterns.SelectMany(x => new[] { x.Item1, x.Item2 }));
        }
    }
}
