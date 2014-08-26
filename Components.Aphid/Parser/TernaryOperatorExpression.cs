using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class TernaryOperatorExpression : Expression, IParentNode
    {
        public AphidTokenType Operator { get; set; }

        public Expression FirstOperand { get; set; }

        public Expression SecondOperand { get; set; }

        public Expression ThirdOperand { get; set; }

        public TernaryOperatorExpression(
            AphidTokenType op,
            Expression firstOperand,
            Expression secondOperand,
            Expression thirdOperand)
        {
            Operator = op;
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            ThirdOperand = thirdOperand;
        }

        public IEnumerable<Expression> GetChildren()
        {
            return new[] { FirstOperand, SecondOperand, ThirdOperand };
        }
    }
}
