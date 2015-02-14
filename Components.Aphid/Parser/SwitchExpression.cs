﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class SwitchExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.SwitchExpression; }
        }

        public AphidExpression Expression { get; set; }

        public List<SwitchCase> Cases { get; set; }

        public List<AphidExpression> DefaultCase { get; set; }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return new[] { Expression }
                .Concat(Cases.SelectMany(x => x.GetChildren()))
                .Concat(DefaultCase);
        }
    }
}
