namespace Components.BlueRacer
{
    public enum InstructionSubOpcode : byte
    {
        Assign = 0x01,
        Inc_R = 0x20,
        Dec_R = 0x21,
        Out = 0xF1,

    }
}