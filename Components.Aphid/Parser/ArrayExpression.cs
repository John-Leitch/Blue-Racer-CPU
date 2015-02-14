﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class ArrayExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.ArrayExpression; }
        }

        public List<AphidExpression> Elements { get; set; }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return Elements;
        }        
    }
}
