using M68k.Memory;

namespace M68k.CPU
{
    public interface ICPU
    {
        int CFlag { get; }

        int InterruptFlagMask { get; }

        int NFlag { get; }

        int SupervisorFlag { get; }

        int TraceFlag { get; }

        int VFlag { get; }

        int XFlag { get; }

        int ZFlag { get; }

        void CalcFlags(InstructionType type, int s, int d, int r, Size sz);

        void CalcFlagsParam(InstructionType type, int s, int d, int r, int extraParam, Size sz);

        void ClrFlags(int flags);

        void DecrementAddrRegister(int reg, int numBytes);

        DisassembledOperand DisassembleDstEA(int address, int mode, int reg, Size sz);

        DisassembledOperand DisassembleSrcEA(int address, int mode, int reg, Size sz);

        int Execute();

        int FetchPCLong();

        int FetchPCWord();

        int FetchPCWordSigned();

        int GetAddrRegisterByte(int reg);

        int GetAddrRegisterByteSigned(int reg);

        int GetAddrRegisterLong(int reg);

        int GetAddrRegisterWord(int reg);

        int GetAddrRegisterWordSigned(int reg);

        int GetCCRegister();

        int GetDataRegisterByte(int reg);

        int GetDataRegisterByteSigned(int reg);

        int GetDataRegisterLong(int reg);

        int GetDataRegisterWord(int reg);

        int GetDataRegisterWordSigned(int reg);

        IInstruction GetInstructionAt(int address);

        IInstruction GetInstructionFor(int opcode);

        int GetInterruptLevel();

        int GetPC();

        int GetSR();

        int GetSSP();

        int GetUSP();

        void IncrementAddrRegister(int reg, int numBytes);

        bool IsFlagSet(int flag);

        bool IsSupervisorMode();

        int PopLong();

        int PopWord();

        void PushLong(int value);

        void PushWord(int value);

        void RaiseException(int vector);

        void RaiseInterrupt(int priority);

        void RaiseSRException();

        int ReadMemoryByte(int addr);

        int ReadMemoryByteSigned(int addr);

        int ReadMemoryLong(int addr);

        int ReadMemoryWord(int addr);

        int ReadMemoryWordSigned(int addr);

        void Reset();

        void ResetExternal();

        IOperand ResolveDstEA(int mode, int reg, Size sz);

        IOperand ResolveSrcEA(int mode, int reg, Size sz);

        void SetAddressSpace(IAddressSpace memory);

        void SetAddrRegisterByte(int reg, int value);

        void SetAddrRegisterLong(int reg, int value);

        void SetAddrRegisterWord(int reg, int value);

        void SetCCRegister(int value);

        void SetDataRegisterByte(int reg, int value);

        void SetDataRegisterLong(int reg, int value);

        void SetDataRegisterWord(int reg, int value);

        void SetFlags(int flags);

        void SetPC(int address);

        void SetSR(int value);

        void SetSR2(int value);

        void SetSSP(int address);

        void SetUSP(int address);

        void StopNow();

        bool TestCC(int cc);

        void WriteMemoryByte(int addr, int value);

        void WriteMemoryLong(int addr, int value);

        void WriteMemoryWord(int addr, int value);
    }
}