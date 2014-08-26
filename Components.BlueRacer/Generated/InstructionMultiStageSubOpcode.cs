namespace Components.BlueRacer
{
    public enum InstructionMultiStageSubOpcode : byte
    {
        Push_R = 0x01,
        Pop_R = 0x02,
        Read = 0x10,
        Write = 0x11,
        Add_R_R = 0x20,
        Sub_R_R = 0x21,
        Mul_R_R = 0x22,
        Div_R_R = 0x23,
        And_R_R = 0x24,
        Or_R_R = 0x25,
        Xor_R_R = 0x26,
        LeftShift_R_R = 0x30,
        RightShift_R_R = 0x31,
        Call_R = 0xE0,
        Return = 0xF0,

    }
}