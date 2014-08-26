namespace Components.BlueRacer
{
    public enum InstructionSubOpcode : byte
    {
        Assign = 0x01,
        Inc_R = 0x20,
        Dec_R = 0x21,
        Eq_R_R = 0x50,
        NotEq_R_R = 0x51,
        LessThan_R_R = 0x52,
        LessThanEq_R_R = 0x53,
        GreaterThan_R_R = 0x54,
        GreaterThanEq_R_R = 0x55,
        In = 0xF0,
        Out = 0xF1,

    }
}