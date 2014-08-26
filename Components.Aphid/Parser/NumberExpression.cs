using System;

namespace Components.Aphid.Parser
{
    public class NumberExpression : Expression
    {
        public decimal Value { get; set; }

        public NumberExpression(decimal value)
        {
            Value = value;
        }

        public override string ToString ()
        {
            return Value.ToString ();
        }
    }
}

