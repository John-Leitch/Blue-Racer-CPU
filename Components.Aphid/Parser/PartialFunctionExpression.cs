﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class PartialFunctionExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.PartialFunctionExpression; }
        }

        public CallExpression Call { get; set; }

        public PartialFunctionExpression()
        {
        }

        public PartialFunctionExpression(CallExpression call)
        {
            Call = call;
        }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return new[] { Call };
        }
    }
}
