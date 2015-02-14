﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Aphid.Parser
{
    public class FunctionExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.FunctionExpression; }
        }

        public List<AphidExpression> Args { get; set; }

        public List<AphidExpression> Body { get; set; }

        public override string ToString ()
        {
            return string.Format ("@({0}) {{ {1} }}", 
                Args.Select(x => x.ToString()).DefaultIfEmpty().Aggregate((x, y) => x + " " + y), 
                Body.Select(x => x.ToString()).DefaultIfEmpty().Aggregate((x, y) => x + " " + y));
        }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return Args.Concat(Body);
        }
    }
}
