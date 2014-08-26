namespace Components.BlueRacer
{
    public enum InstructionMultiStageSubOpcode : byte
    {
        Push_R = 0x01,
        Pop_R = 0x02,
        Read = 0x10,
        Write = 0x11,
        And_R_R = 0x26,
        Or_R_R = 0x27,
        Call_R = 0xE0,
        Return = 0xF0,

    }
}