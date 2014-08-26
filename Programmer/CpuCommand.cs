namespace Processor
{
    public enum CpuCommand : uint
    {
        Nop = 0x00000000,
        Break = 0x52300000,
        Continue = 0x52300001,
        Restart = 0x52300002,
        Reset = 0x52300005,
        ReadAddress = 0x52300010,
        Read4 = 0x52300020,
        WriteAddress = 0x52300030,
        Write4 = 0x52300040,
        GetProgramCounter = 0x52300050,
        GetInstructionAddress = 0x52300051,
        GetError = 0x52300052,
        GetErrorCode = 0x52300053,
        GetOpcode = 0x52300054,
        GetOperands = 0x52300055,
        GetRegisters = 0x52300056,
        GetPageFlags = 0x52300057,
        SetPageFlags = 0x52300100
    }
}
