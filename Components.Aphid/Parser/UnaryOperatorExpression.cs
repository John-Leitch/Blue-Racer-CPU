﻿using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Aphid.Parser
{
    public class UnaryOperatorExpression : AphidExpression, IParentNode
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.UnaryOperatorExpression; }
        }

        public AphidTokenType Operator { get; set; }

        public AphidExpression Operand { get; set; }

        public bool IsPostfix { get; set; }

        public UnaryOperatorExpression(AphidTokenType op, AphidExpression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override string ToString ()
        {
            return IsPostfix ? 
                string.Format ("{0} {1}", Operand, Operator) :
                    string.Format ("{0} {1}", Operator, Operand);
        }

        public IEnumerable<AphidExpression> GetChildren()
        {
            return new[] { Operand };
        }
    }
}
