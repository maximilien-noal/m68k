using M68k.Memory;

namespace M68k.CPU
{
    public interface ICPU
    {
        uint CFlag { get; }

        uint InterruptFlagMask { get; }

        uint NFlag { get; }

        uint SupervisorFlag { get; }

        uint TraceFlag { get; }

        uint VFlag { get; }

        uint XFlag { get; }

        uint ZFlag { get; }

        void CalcFlags(InstructionType type, uint s, uint d, uint r, Size sz);

        void CalcFlagsParam(InstructionType type, uint s, uint d, uint r, uint extraParam, Size sz);

        void ClrFlags(uint flags);

        void DecrementAddrRegister(uint reg, uint numBytes);

        DisassembledOperand DisassembleDstEA(uint address, uint mode, uint reg, Size sz);

        DisassembledOperand DisassembleSrcEA(uint address, uint mode, uint reg, Size sz);

        uint Execute();

        uint FetchPCLong();

        uint FetchPCWord();

        uint FetchPCWordSigned();

        uint GetAddrRegisterByte(uint reg);

        uint GetAddrRegisterByteSigned(uint reg);

        uint GetAddrRegisterLong(uint reg);

        uint GetAddrRegisterWord(uint reg);

        uint GetAddrRegisterWordSigned(uint reg);

        uint GetCCRegister();

        uint GetDataRegisterByte(uint reg);

        uint GetDataRegisterByteSigned(uint reg);

        uint GetDataRegisterLong(uint reg);

        uint GetDataRegisterWord(uint reg);

        uint GetDataRegisterWordSigned(uint reg);

        IInstruction GetInstructionAt(uint address);

        IInstruction GetInstructionFor(uint opcode);

        uint GetInterruptLevel();

        uint GetPC();

        uint GetSR();

        uint GetSSP();

        uint GetUSP();

        void IncrementAddrRegister(uint reg, uint numBytes);

        bool IsFlagSet(uint flag);

        bool IsSupervisorMode();

        uint PopLong();

        uint PopWord();

        void PushLong(uint value);

        void PushWord(uint value);

        void RaiseException(uint vector);

        void RaiseInterrupt(uint priority);

        void RaiseSRException();

        uint ReadMemoryByte(uint addr);

        uint ReadMemoryByteSigned(uint addr);

        uint ReadMemoryLong(uint addr);

        uint ReadMemoryWord(uint addr);

        uint ReadMemoryWordSigned(uint addr);

        void Reset();

        void ResetExternal();

        IOperand ResolveDstEA(uint mode, uint reg, Size sz);

        IOperand ResolveSrcEA(uint mode, uint reg, Size sz);

        void SetAddressSpace(IAddressSpace memory);

        void SetAddrRegisterByte(uint reg, uint value);

        void SetAddrRegisterLong(uint reg, uint value);

        void SetAddrRegisterWord(uint reg, uint value);

        void SetCCRegister(uint value);

        void SetDataRegisterByte(uint reg, uint value);

        void SetDataRegisterLong(uint reg, uint value);

        void SetDataRegisterWord(uint reg, uint value);

        void SetFlags(uint flags);

        void SetPC(uint address);

        void SetSR(uint value);

        void SetSR2(uint value);

        void SetSSP(uint address);

        void SetUSP(uint address);

        void StopNow();

        bool TestCC(uint cc);

        void WriteMemoryByte(uint addr, uint value);

        void WriteMemoryLong(uint addr, uint value);

        void WriteMemoryWord(uint addr, uint value);
    }
}