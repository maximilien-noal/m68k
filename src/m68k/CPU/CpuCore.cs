using M68k.Memory;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace M68k.CPU
{
    public abstract class CpuCore : ICPU
    {
        private const int C_FLAG = 1;

        private const int INTERRUPT_FLAGS_MASK = 0x0700;

        private const int N_FLAG = 8;

        private const int SUPERVISOR_FLAG = 0x2000;

        private const int TRACE_FLAG = 0x8000;

        private const int V_FLAG = 2;

        private const int X_FLAG = 16;

        private const int Z_FLAG = 4;

        private readonly List<int> addressRegisters = new int[8].ToList();

        private readonly List<int> dataRegisters = new int[8].ToList();

        private IOperand[] dstHandlers;

        private int reg_sr;

        private int reg_ssp;

        private int reg_usp;

        private IOperand[] srcHandlers;

        public List<int> AdressRegisters { get => addressRegisters; }

        public int CFlag => C_FLAG;

        public int CurrentInstructionAddress { get; set; }

        public List<int> DataRegisters { get => dataRegisters; }

        public StringBuilder DisasmBuffer { get; set; }

        public IOperand DstEAHandler { get; set; }

        public int InterruptFlagMask => INTERRUPT_FLAGS_MASK;

        public IAddressSpace Memory { get; set; }

        public int NFlag => N_FLAG;

        public int RegPc { get; set; }

        public IOperand SrcEAHandler { get; set; }

        public int SupervisorFlag => SUPERVISOR_FLAG;

        public int TraceFlag => TRACE_FLAG;

        public int VFlag => V_FLAG;

        public int XFlag => X_FLAG;

        public int ZFlag => Z_FLAG;

        public virtual void CalcFlags(InstructionType type, int src, int dst, int result, Size sz)
        {
            CalcFlagsParam(type, src, dst, result, 0, sz);
        }

        public virtual void CalcFlagsParam(InstructionType type, int src, int dst, int result, int extraParam, Size sz)
        {
            bool Sm = (src & sz.MSB) != 0;
            bool Dm = (dst & sz.MSB) != 0;
            bool Rm = (result & sz.MSB) != 0;
            switch (type)
            {
                case InstructionType.ADD:
                    {
                        bool Zm = (result & sz.Mask) == 0;
                        if ((Sm && Dm && !Rm) || (!Sm && !Dm && Rm))
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if ((Sm && Dm) || (!Rm && Dm) || (Sm && !Rm))
                        {
                            reg_sr |= (C_FLAG | X_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG | X_FLAG);
                        }

                        if (Zm)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.ADDX:
                    {
                        if ((Sm && Dm && !Rm) || (!Sm && !Dm && Rm))
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if ((Sm && Dm) || (!Rm && Dm) || (Sm && !Rm))
                        {
                            reg_sr |= (C_FLAG | X_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG | X_FLAG);
                        }

                        if (result != 0)
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.ASL:
                    {
                        if (src != 0)
                        {
                            if (dst != 0)
                            {
                                reg_sr |= (C_FLAG | X_FLAG);
                            }
                            else
                            {
                                reg_sr &= ~(C_FLAG | X_FLAG);
                            }
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        if (extraParam != 0)
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~V_FLAG;
                        }

                        break;
                    }

                case InstructionType.ASR:
                    {
                        if (src != 0)
                        {
                            if (dst != 0)
                            {
                                reg_sr |= (C_FLAG | X_FLAG);
                            }
                            else
                            {
                                reg_sr &= ~(C_FLAG | X_FLAG);
                            }
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        reg_sr &= ~V_FLAG;
                        break;
                    }

                case InstructionType.CMP:
                    {
                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if ((!Sm && Dm && !Rm) || (Sm && !Dm && Rm))
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if ((Sm && !Dm) || (Rm && !Dm) || (Sm && Rm))
                        {
                            reg_sr |= C_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.LSL:
                case InstructionType.LSR:
                case InstructionType.ROXL:
                case InstructionType.ROXR:
                    {
                        if (src > 0)
                        {
                            if (dst != 0)
                            {
                                reg_sr |= (C_FLAG | X_FLAG);
                            }
                            else
                            {
                                reg_sr &= ~(C_FLAG | X_FLAG);
                            }
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        reg_sr &= ~(V_FLAG);
                        break;
                    }

                case InstructionType.AND:
                case InstructionType.EOR:
                case InstructionType.MOVE:
                case InstructionType.NOT:
                case InstructionType.OR:
                    {
                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        reg_sr &= ~(V_FLAG | C_FLAG);
                        break;
                    }

                case InstructionType.NEG:
                    {
                        if (Sm && Rm)
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                            reg_sr &= ~(X_FLAG | C_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                            reg_sr |= (X_FLAG | C_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.NEGX:
                    {
                        if (Sm && Rm)
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if (Sm || Rm)
                        {
                            reg_sr |= (X_FLAG | C_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(X_FLAG | C_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.ROL:
                case InstructionType.ROR:
                    {
                        if (src > 0)
                        {
                            if (dst != 0)
                            {
                                reg_sr |= C_FLAG;
                            }
                            else
                            {
                                reg_sr &= ~(C_FLAG);
                            }
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG);
                        }

                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        reg_sr &= ~(V_FLAG);
                        break;
                    }

                case InstructionType.SUB:
                    {
                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if ((!Sm && Dm && !Rm) || (Sm && !Dm && Rm))
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if ((Sm && !Dm) || (Rm && !Dm) || (Sm && Rm))
                        {
                            reg_sr |= (C_FLAG | X_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG | X_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.SUBX:
                    {
                        if (result != 0)
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if ((!Sm && Dm && !Rm) || (Sm && !Dm && Rm))
                        {
                            reg_sr |= V_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(V_FLAG);
                        }

                        if ((Sm && !Dm) || (Rm && !Dm) || (Sm && Rm))
                        {
                            reg_sr |= (C_FLAG | X_FLAG);
                        }
                        else
                        {
                            reg_sr &= ~(C_FLAG | X_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        break;
                    }

                case InstructionType.SWAP:
                    {
                        if (result == 0)
                        {
                            reg_sr |= Z_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(Z_FLAG);
                        }

                        if (Rm)
                        {
                            reg_sr |= N_FLAG;
                        }
                        else
                        {
                            reg_sr &= ~(N_FLAG);
                        }

                        reg_sr &= ~(V_FLAG);
                        reg_sr &= ~(C_FLAG);
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("No flags handled for " + type);
                    }
            }
        }

        public virtual void ClrFlags(int flags)
        {
            reg_sr &= ~(flags & 0x00ff);
        }

        public virtual void DecrementAddrRegister(int reg, int numBytes)
        {
            AdressRegisters[reg] -= numBytes;
        }

        public virtual DisassembledOperand DisassembleDstEA(int address, int mode, int reg, Size sz)
        {
            return DisassembleEA(address, mode, reg, sz, false);
        }

        public virtual DisassembledOperand DisassembleSrcEA(int address, int mode, int reg, Size sz)
        {
            return DisassembleEA(address, mode, reg, sz, true);
        }

        public virtual int Execute()
        {
            throw new NotImplementedException($"Use {nameof(MC68000)}, not {nameof(CpuCore)}");
        }

        public virtual int FetchPCLong()
        {
            int value = ReadMemoryLong(RegPc);
            RegPc += 4;
            return value;
        }

        public virtual int FetchPCWord()
        {
            int value = ReadMemoryWord(RegPc);
            RegPc += 2;
            return value;
        }

        public virtual int FetchPCWordSigned()
        {
            int value = ReadMemoryWordSigned(RegPc);
            RegPc += 2;
            return value;
        }

        public virtual int GetAddrRegisterByte(int reg)
        {
            return AdressRegisters[reg] & 0x00ff;
        }

        public virtual int GetAddrRegisterByteSigned(int reg)
        {
            return SignExtendByte(AdressRegisters[reg]);
        }

        public virtual int GetAddrRegisterLong(int reg)
        {
            return AdressRegisters[reg];
        }

        public virtual int GetAddrRegisterWord(int reg)
        {
            return AdressRegisters[reg] & 0x0000ffff;
        }

        public virtual int GetAddrRegisterWordSigned(int reg)
        {
            return SignExtendWord(AdressRegisters[reg]);
        }

        public virtual int GetCCRegister()
        {
            return reg_sr & 0x00ff;
        }

        public virtual int GetDataRegisterByte(int reg)
        {
            return DataRegisters[reg] & 0x00ff;
        }

        public virtual int GetDataRegisterByteSigned(int reg)
        {
            return SignExtendByte(DataRegisters[reg]);
        }

        public virtual int GetDataRegisterLong(int reg)
        {
            return DataRegisters[reg];
        }

        public virtual int GetDataRegisterWord(int reg)
        {
            return DataRegisters[reg] & 0x0000ffff;
        }

        public virtual int GetDataRegisterWordSigned(int reg)
        {
            return SignExtendWord(DataRegisters[reg]);
        }

        public virtual IInstruction GetInstructionAt(int address)
        {
            throw new NotImplementedException($"Use {nameof(MC68000)}, not {nameof(CpuCore)}");
        }

        public virtual IInstruction GetInstructionFor(int opcode)
        {
            throw new NotImplementedException($"Use {nameof(MC68000)}, not {nameof(CpuCore)}");
        }

        public virtual int GetInterruptLevel()
        {
            return (reg_sr >> 8) & 0x07;
        }

        public virtual int GetPC()
        {
            return RegPc;
        }

        public virtual int GetSR()
        {
            return reg_sr;
        }

        public virtual int GetSSP()
        {
            return reg_ssp;
        }

        public virtual int GetUSP()
        {
            return reg_usp;
        }

        public virtual void IncrementAddrRegister(int reg, int numBytes)
        {
            AdressRegisters[reg] += numBytes;
        }

        public virtual bool IsFlagSet(int flag)
        {
            return ((reg_sr & flag) == flag);
        }

        public virtual bool IsSupervisorMode()
        {
            return (reg_sr & SUPERVISOR_FLAG) == SUPERVISOR_FLAG;
        }

        public virtual int PopLong()
        {
            int val = ReadMemoryLong(AdressRegisters[7]);
            AdressRegisters[7] += 4;
            return val;
        }

        public virtual int PopWord()
        {
            int val = ReadMemoryWord(AdressRegisters[7]);
            AdressRegisters[7] += 2;
            return val;
        }

        public virtual void PushLong(int value)
        {
            AdressRegisters[7] -= 4;
            WriteMemoryLong(AdressRegisters[7], value);
        }

        public virtual void PushWord(int value)
        {
            AdressRegisters[7] -= 2;
            WriteMemoryWord(AdressRegisters[7], value);
        }

        public virtual void RaiseException(int vector)
        {
            int address = (vector & 0x00ff) << 2;
            int old_sr = reg_sr;
            if ((reg_sr & SUPERVISOR_FLAG) == 0)
            {
                reg_sr |= SUPERVISOR_FLAG;
                reg_usp = AdressRegisters[7];
                AdressRegisters[7] = reg_ssp;
            }

            PushLong(RegPc);
            PushWord(old_sr);
            reg_sr &= ~(TRACE_FLAG);
            int xaddress = ReadMemoryLong(address);
            if (xaddress == 0)
            {
                xaddress = ReadMemoryLong(0x003c);
                if (xaddress == 0)
                {
                    throw new CpuException("Interrupt vector not set for uninitialised interrupt vector while trapping uninitialised vector " + vector);
                }
            }

            RegPc = xaddress;
        }

        public virtual void RaiseInterrupt(int priority)
        {
            if (priority == 0)
                return;
            priority &= 0x07;
            if (priority > GetInterruptLevel())
            {
                RaiseException(priority + 24);
                SetInterruptLevel(priority);
            }
        }

        public virtual void RaiseSRException()
        {
            int address = 32;
            int old_sr = reg_sr;
            if ((reg_sr & SUPERVISOR_FLAG) == 0)
            {
                reg_sr |= SUPERVISOR_FLAG;
                reg_usp = AdressRegisters[7];
                AdressRegisters[7] = reg_ssp;
            }

            PushLong(CurrentInstructionAddress);
            PushWord(old_sr);
            int xaddress = ReadMemoryLong(address);
            if (xaddress == 0)
            {
                xaddress = ReadMemoryLong(0x003c);
                if (xaddress == 0)
                {
                    throw new CpuException("Interrupt vector not set for uninitialised interrupt vector while trapping uninitialised vector 8");
                }
            }

            RegPc = xaddress;
        }

        public virtual int ReadMemoryByte(int addr)
        {
            return Memory.ReadByte(addr);
        }

        public virtual int ReadMemoryByteSigned(int addr)
        {
            return SignExtendByte(Memory.ReadByte(addr));
        }

        public virtual int ReadMemoryLong(int addr)
        {
            return (int)Memory.ReadLong(addr);
        }

        public virtual int ReadMemoryWord(int addr)
        {
            return Memory.ReadWord(addr);
        }

        public virtual int ReadMemoryWordSigned(int addr)
        {
            return SignExtendWord(Memory.ReadWord(addr));
        }

        public virtual void Reset()
        {
            reg_ssp = (int)Memory.ReadLong(0);
            AdressRegisters[7] = reg_ssp;

            RegPc = (int)Memory.ReadLong(4);
            reg_sr = 0x2700;
        }

        public void ResetExternal()
        {
            // Method intentionally left empty.
        }

        public virtual IOperand ResolveDstEA(int mode, int reg, Size size)
        {
            if (mode < 7)
            {
                DstEAHandler = dstHandlers[mode];
            }
            else
            {
                DstEAHandler = dstHandlers[mode + reg];
            }

            DstEAHandler.Init(reg, size);
            return DstEAHandler;
        }

        public virtual IOperand ResolveSrcEA(int mode, int reg, Size size)
        {
            if (mode < 7)
            {
                SrcEAHandler = srcHandlers[mode];
            }
            else
            {
                SrcEAHandler = srcHandlers[mode + reg];
            }

            SrcEAHandler.Init(reg, size);
            return SrcEAHandler;
        }

        public virtual void SetAddressSpace(IAddressSpace memory)
        {
            this.Memory = memory;
        }

        public virtual void SetAddrRegisterByte(int reg, int value)
        {
            AdressRegisters[reg] = (int)(AdressRegisters[reg] & 0xffffff00) | (value & 0x00ff);
            if (reg == 7)
            {
                if (IsSupervisorMode())
                {
                    reg_ssp = AdressRegisters[reg];
                }
                else
                {
                    reg_usp = AdressRegisters[reg];
                }
            }
        }

        public virtual void SetAddrRegisterLong(int reg, int value)
        {
            AdressRegisters[reg] = value;
            if (reg == 7)
            {
                if (IsSupervisorMode())
                {
                    reg_ssp = value;
                }
                else
                {
                    reg_usp = value;
                }
            }
        }

        public virtual void SetAddrRegisterWord(int reg, int value)
        {
            AdressRegisters[reg] = (int)(AdressRegisters[reg] & 0xffff0000) | (value & 0x0000ffff);
            if (reg == 7)
            {
                if (IsSupervisorMode())
                {
                    reg_ssp = AdressRegisters[reg];
                }
                else
                {
                    reg_usp = AdressRegisters[reg];
                }
            }
        }

        public virtual void SetCCRegister(int value)
        {
            reg_sr = (reg_sr & 0xff00) | (value & 0x00ff);
        }

        public virtual void SetDataRegisterByte(int reg, int value)
        {
            DataRegisters[reg] = (int)(DataRegisters[reg] & 0xffffff00) | (value & 0x00ff);
        }

        public virtual void SetDataRegisterLong(int reg, int value)
        {
            DataRegisters[reg] = value;
        }

        public virtual void SetDataRegisterWord(int reg, int value)
        {
            DataRegisters[reg] = (int)(DataRegisters[reg] & 0xffff0000) | (value & 0x0000ffff);
        }

        public virtual void SetFlags(int flags)
        {
            reg_sr |= (flags & 0x00ff);
        }

        public virtual void SetPC(int address)
        {
            RegPc = address;
        }

        public virtual void SetSR(int value)
        {
            if (((reg_sr & SUPERVISOR_FLAG) ^ (value & SUPERVISOR_FLAG)) != 0)
            {
                if ((value & SUPERVISOR_FLAG) != 0)
                {
                    reg_usp = AdressRegisters[7];
                    AdressRegisters[7] = reg_ssp;
                }
                else
                {
                    reg_ssp = AdressRegisters[7];
                    AdressRegisters[7] = reg_usp;
                }
            }

            reg_sr = value;
        }

        public virtual void SetSR2(int value)
        {
            reg_sr = value;
            if ((reg_sr & SUPERVISOR_FLAG) == 0)
            {
                reg_ssp = AdressRegisters[7];
                AdressRegisters[7] = reg_usp;
            }
        }

        public virtual void SetSSP(int address)
        {
            reg_ssp = address;
            if (IsSupervisorMode())
                AdressRegisters[7] = reg_ssp;
        }

        public virtual void SetSupervisorMode(bool enable)
        {
            if (enable)
            {
                int old_sr = reg_sr;
                if ((reg_sr & SUPERVISOR_FLAG) == 0)
                {
                    reg_sr |= SUPERVISOR_FLAG;
                    reg_usp = AdressRegisters[7];
                    AdressRegisters[7] = reg_ssp;
                }

                PushLong(RegPc);
                PushWord(old_sr);
            }
            else
            {
                if ((reg_sr & SUPERVISOR_FLAG) != 0)
                {
                    reg_sr = PopWord();
                    RegPc = PopLong();
                    reg_ssp = AdressRegisters[7];
                    AdressRegisters[7] = reg_usp;
                }
            }
        }

        public virtual void SetUSP(int address)
        {
            reg_usp = address;
            if (!IsSupervisorMode())
            {
                AdressRegisters[7] = reg_usp;
            }
        }

        public virtual void StopNow()
        {
        }

        public virtual bool TestCC(int cc)
        {
            int ccr = reg_sr & 0x001f;
            switch (cc)
            {
                case 0:
                    {
                        return true;
                    }

                case 1:
                    {
                        return false;
                    }

                case 2:
                    {
                        return ((ccr & (C_FLAG | Z_FLAG)) == 0);
                    }

                case 3:
                    {
                        return ((ccr & (C_FLAG | Z_FLAG)) != 0);
                    }

                case 4:
                    {
                        return ((ccr & C_FLAG) == 0);
                    }

                case 5:
                    {
                        return ((ccr & C_FLAG) != 0);
                    }

                case 6:
                    {
                        return ((ccr & Z_FLAG) == 0);
                    }

                case 7:
                    {
                        return ((ccr & Z_FLAG) != 0);
                    }

                case 8:
                    {
                        return ((ccr & V_FLAG) == 0);
                    }

                case 9:
                    {
                        return ((ccr & V_FLAG) != 0);
                    }

                case 10:
                    {
                        return ((ccr & N_FLAG) == 0);
                    }

                case 11:
                    {
                        return ((ccr & N_FLAG) != 0);
                    }

                case 12:
                    {
                        int v = ccr & (N_FLAG | V_FLAG);
                        return (v == 0 || v == (N_FLAG | V_FLAG));
                    }

                case 13:
                    {
                        int v = ccr & (N_FLAG | V_FLAG);
                        return (v == N_FLAG || v == V_FLAG);
                    }

                case 14:
                    {
                        int v = ccr & (N_FLAG | V_FLAG | Z_FLAG);
                        return (v == 0 || v == (N_FLAG | V_FLAG));
                    }

                case 15:
                    {
                        int v = ccr & (N_FLAG | V_FLAG | Z_FLAG);
                        return ((v & Z_FLAG) != 0 || (v == N_FLAG) || (v == V_FLAG));
                    }
            }

            throw new ArgumentException("Invalid Condition Code value!");
        }

        public virtual void WriteMemoryByte(int addr, int value)
        {
            Memory.WriteByte(addr, value);
        }

        public virtual void WriteMemoryLong(int addr, int value)
        {
            Memory.WriteLong(addr, (uint)value);
        }

        public virtual void WriteMemoryWord(int addr, int value)
        {
            Memory.WriteWord(addr, value);
        }

        protected virtual DisassembledOperand DisassembleEA(int address, int mode, int reg, Size sz, bool isSrc)
        {
            int bytes_read = 0;
            int mem = 0;
            DisasmBuffer.Clear();
            switch (mode)
            {
                case 0:
                    {
                        DisasmBuffer.Append("d").Append(reg);
                        break;
                    }

                case 1:
                    {
                        DisasmBuffer.Append("a").Append(reg);
                        break;
                    }

                case 2:
                    {
                        DisasmBuffer.Append("(a").Append(reg).Append(")");
                        break;
                    }

                case 3:
                    {
                        DisasmBuffer.Append("(a").Append(reg).Append(")+");
                        break;
                    }

                case 4:
                    {
                        DisasmBuffer.Append("-(a").Append(reg).Append(")");
                        break;
                    }

                case 5:
                    {
                        mem = ReadMemoryWordSigned(address);
                        DisasmBuffer.Append($"{((short)mem).ToString("x4", CultureInfo.InvariantCulture)}(a{reg})");
                        bytes_read = 2;
                        break;
                    }

                case 6:
                    {
                        mem = ReadMemoryWord(address);
                        int dis = SignExtendByte(mem);
                        DisasmBuffer.Append($"{dis.ToString("x4", CultureInfo.InvariantCulture)}(a{reg},");
                        DisasmBuffer.Append((mem & 0x8000) != 0 ? "a" : "d").Append((mem >> 12) & 0x07).Append((mem & 0x0800) != 0 ? ".l" : ".w").Append(")");
                        bytes_read = 2;
                        break;
                    }

                case 7:
                    {
                        switch (reg)
                        {
                            case 0:
                                {
                                    mem = ReadMemoryWord(address);
                                    DisasmBuffer.Append($"{mem.ToString("x4", CultureInfo.InvariantCulture)}");
                                    bytes_read = 2;
                                    break;
                                }

                            case 1:
                                {
                                    mem = ReadMemoryLong(address);
                                    DisasmBuffer.Append($"{mem.ToString("x8", CultureInfo.InvariantCulture)}");
                                    bytes_read = 4;
                                    break;
                                }

                            case 2:
                                {
                                    mem = ReadMemoryWordSigned(address);
                                    DisasmBuffer.Append($"${((short)mem).ToString("x4", CultureInfo.InvariantCulture)}(pc)");
                                    bytes_read = 2;
                                    break;
                                }

                            case 3:
                                {
                                    mem = ReadMemoryWord(address);
                                    int dis = SignExtendByte(mem);
                                    DisasmBuffer.Append($"${((byte)dis).ToString("x2", CultureInfo.InvariantCulture)}(pc,");
                                    DisasmBuffer.Append(((mem & 0x8000) != 0 ? "a" : "d")).Append((mem >> 12) & 0x07).Append(((mem & 0x0800) != 0 ? ".l" : ".w")).Append(")");
                                    bytes_read = 2;
                                    break;
                                }

                            case 4:
                                {
                                    if (isSrc)
                                    {
                                        if (sz.Equals(Size.SizeLong))
                                        {
                                            mem = ReadMemoryLong(address);
                                            bytes_read = 4;
                                            DisasmBuffer.Append($"#${mem.ToString("x8", CultureInfo.InvariantCulture)}");
                                        }
                                        else
                                        {
                                            mem = ReadMemoryWord(address);
                                            bytes_read = 2;
                                            DisasmBuffer.Append($"#${mem.ToString("x4", CultureInfo.InvariantCulture)}");
                                            if (sz.Equals(Size.Byte))
                                            {
                                                mem &= 0x00ff;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (sz.Equals(Size.Byte))
                                        {
                                            DisasmBuffer.Append("ccr");
                                        }
                                        else
                                        {
                                            DisasmBuffer.Append("sr");
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    throw new ArgumentException("Invalid reg specified for mode 7: " + reg);
                                }
                        }

                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Invalid mode specified: " + mode);
                    }
            }

            return new DisassembledOperand(DisasmBuffer.ToString(), bytes_read, mem);
        }

        protected virtual void InitEAHandlers()
        {
            srcHandlers = new IOperand[12];
            dstHandlers = new IOperand[12];
            srcHandlers[0] = new DataRegisterOperand(this);
            srcHandlers[1] = new AddressRegisterOperand(this);
            srcHandlers[2] = new AddressRegisterIndirectOperand(this);
            srcHandlers[3] = new AddressRegisterPostIncOperand(this);
            srcHandlers[4] = new AddressRegisterPreDecOperand(this);
            srcHandlers[5] = new AddressRegisterWithDisplacementOperand(this);
            srcHandlers[6] = new AddressRegisterWithIndexOperand(this);
            srcHandlers[7] = new AbsoluteShortOperand(this);
            srcHandlers[8] = new AbsoluteLongOperand(this);
            srcHandlers[9] = new PCWithDisplacementOperand(this);
            srcHandlers[10] = new PCWithIndexOperand(this);
            srcHandlers[11] = new ImmediateOperand(this);
            dstHandlers[0] = new DataRegisterOperand(this);
            dstHandlers[1] = new AddressRegisterOperand(this);
            dstHandlers[2] = new AddressRegisterIndirectOperand(this);
            dstHandlers[3] = new AddressRegisterPostIncOperand(this);
            dstHandlers[4] = new AddressRegisterPreDecOperand(this);
            dstHandlers[5] = new AddressRegisterWithDisplacementOperand(this);
            dstHandlers[6] = new AddressRegisterWithIndexOperand(this);
            dstHandlers[7] = new AbsoluteShortOperand(this);
            dstHandlers[8] = new AbsoluteLongOperand(this);
            dstHandlers[9] = new PCWithDisplacementOperand(this);
            dstHandlers[10] = new PCWithIndexOperand(this);
            dstHandlers[11] = new StatusRegisterOperand(this);
        }

        protected virtual void SetInterruptLevel(int level)
        {
            reg_sr &= ~(INTERRUPT_FLAGS_MASK);
            reg_sr |= (level & 0x07) << 8;
        }

        protected virtual int SignExtendByte(int value)
        {
            if ((value & 0x80) == 0x80)
            {
                unchecked
                {
                    value |= (int)0xffffff00;
                }
            }
            else
            {
                value &= 0x000000ff;
            }

            return value;
        }

        protected virtual int SignExtendWord(int value)
        {
            if ((value & 0x8000) == 0x8000)
            {
                unchecked
                {
                    value |= (int)0xffff0000;
                }
            }
            else
            {
                value &= 0x0000ffff;
            }

            return value;
        }

        private class AbsoluteLongOperand : IOperand
        {
            protected readonly int index = 8;

            protected int address;

            protected Size size;

            private readonly ICPU cpu;

            public AbsoluteLongOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 16 : 12);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.FetchPCLong();
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"${address.ToString("x", CultureInfo.InvariantCulture)}.l";
            }
        }

        private class AbsoluteShortOperand : IOperand
        {
            protected readonly int index = 7;

            protected int address;

            protected Size size;

            private readonly ICPU cpu;

            public AbsoluteShortOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.FetchPCWordSigned();
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"${address.ToString("x", CultureInfo.InvariantCulture)}.w";
            }
        }

        private class AddressRegisterIndirectOperand : IOperand
        {
            protected readonly int index = 2;

            protected int address;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public AddressRegisterIndirectOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 8 : 4);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                address = cpu.GetAddrRegisterLong(regNumber);
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"(a{regNumber})";
            }
        }

        private class AddressRegisterOperand : IOperand
        {
            protected readonly int index = 1;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public AddressRegisterOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.GetAddrRegisterByte(regNumber);
            }

            public virtual int GetByteSigned()
            {
                return cpu.GetAddrRegisterByteSigned(regNumber);
            }

            public virtual int GetComputedAddress()
            {
                throw new Exception("Address Register direct has no computed address");
            }

            public virtual int GetLong()
            {
                return cpu.GetAddrRegisterLong(regNumber);
            }

            public virtual int GetTiming()
            {
                return 0;
            }

            public virtual int GetWord()
            {
                return cpu.GetAddrRegisterWord(regNumber);
            }

            public virtual int GetWordSigned()
            {
                return cpu.GetAddrRegisterWordSigned(regNumber);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
            }

            public virtual bool IsRegisterMode()
            {
                return true;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.SetAddrRegisterByte(regNumber, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.SetAddrRegisterLong(regNumber, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.SetAddrRegisterWord(regNumber, value);
            }

            public override string ToString()
            {
                return $"a{regNumber}";
            }
        }

        private class AddressRegisterPostIncOperand : IOperand
        {
            protected readonly int index = 3;

            protected int address;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public AddressRegisterPostIncOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 8 : 4);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                address = cpu.GetAddrRegisterLong(regNumber);
                if (param == 7 && size.ByteCount == 1)
                {
                    cpu.IncrementAddrRegister(regNumber, 2);
                }
                else
                {
                    cpu.IncrementAddrRegister(regNumber, size.ByteCount);
                }
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"(a{regNumber})+";
            }
        }

        private class AddressRegisterPreDecOperand : IOperand
        {
            protected readonly int index = 4;

            protected int address;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public AddressRegisterPreDecOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 10 : 6);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                if (param == 7 && size.ByteCount == 1)
                {
                    cpu.DecrementAddrRegister(regNumber, 2);
                }
                else
                {
                    cpu.DecrementAddrRegister(regNumber, size.ByteCount);
                }

                address = cpu.GetAddrRegisterLong(regNumber);
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryWord(address + 2, value & 0xFFFF);
                cpu.WriteMemoryWord(address, (value >> 16) & 0xFFFF);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"-(a{regNumber})";
            }
        }

        private class AddressRegisterWithDisplacementOperand : IOperand
        {
            protected readonly int index = 5;

            protected int address;

            protected int displacement;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public AddressRegisterWithDisplacementOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                displacement = cpu.FetchPCWordSigned();
                address = cpu.GetAddrRegisterLong(regNumber) + displacement;
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"{displacement.ToString("x", CultureInfo.InvariantCulture)}(a{regNumber})";
            }
        }

        private class AddressRegisterWithIndexOperand : IOperand
        {
            protected readonly int index = 6;

            protected int address;

            protected int displacement;

            protected bool idxIsAddressReg;

            protected int idxRegNumber;

            protected Size idxSize;

            protected int regNumber;

            protected Size size;

            private readonly CpuCore cpu;

            public AddressRegisterWithIndexOperand(CpuCore icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 14 : 10);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                int ext = cpu.FetchPCWordSigned();
                displacement = cpu.SignExtendByte(ext);
                idxRegNumber = (ext >> 12) & 0x07;
                idxSize = ((ext & 0x0800) == 0x0800 ? Size.SizeLong : Size.Word);
                idxIsAddressReg = ((ext & 0x8000) == 0x8000);
                int idxVal;
                if (idxIsAddressReg)
                {
                    if (idxSize.Equals(Size.Word))
                    {
                        idxVal = cpu.GetAddrRegisterWordSigned(idxRegNumber);
                    }
                    else
                    {
                        idxVal = cpu.GetAddrRegisterLong(idxRegNumber);
                    }
                }
                else
                {
                    if (idxSize.Equals(Size.Word))
                    {
                        idxVal = cpu.GetDataRegisterWordSigned(idxRegNumber);
                    }
                    else
                    {
                        idxVal = cpu.GetDataRegisterLong(idxRegNumber);
                    }
                }

                address = cpu.GetAddrRegisterLong(regNumber) + displacement + idxVal;
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                string appended;
                if (idxIsAddressReg)
                {
                    appended = "a";
                }
                else
                {
                    appended = "d";
                }

                return $"{displacement}(a{regNumber},{appended}{idxRegNumber}{idxSize.Ext})";
            }
        }

        private class DataRegisterOperand : IOperand
        {
            protected readonly int index = 0;

            protected int regNumber;

            protected Size size;

            private readonly ICPU cpu;

            public DataRegisterOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.GetDataRegisterByte(regNumber);
            }

            public virtual int GetByteSigned()
            {
                return cpu.GetDataRegisterByteSigned(regNumber);
            }

            public virtual int GetComputedAddress()
            {
                throw new Exception("Data Register has no computed address");
            }

            public virtual int GetLong()
            {
                return cpu.GetDataRegisterLong(regNumber);
            }

            public virtual int GetTiming()
            {
                return 0;
            }

            public virtual int GetWord()
            {
                return cpu.GetDataRegisterWord(regNumber);
            }

            public virtual int GetWordSigned()
            {
                return cpu.GetDataRegisterWordSigned(regNumber);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
            }

            public virtual bool IsRegisterMode()
            {
                return true;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.SetDataRegisterByte(regNumber, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.SetDataRegisterLong(regNumber, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.SetDataRegisterWord(regNumber, value);
            }

            public override string ToString()
            {
                return $"d{regNumber}";
            }
        }

        private class ImmediateOperand : IOperand
        {
            protected readonly int index = 11;

            protected Size size;

            protected int value;

            private readonly CpuCore cpu;

            public ImmediateOperand(CpuCore icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return value & 0x00ff;
            }

            public virtual int GetByteSigned()
            {
                return cpu.SignExtendByte(value);
            }

            public virtual int GetComputedAddress()
            {
                throw new Exception("Immediate addressing has no computed address");
            }

            public virtual int GetLong()
            {
                return value;
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public virtual int GetWord()
            {
                return value & 0x0000ffff;
            }

            public virtual int GetWordSigned()
            {
                return cpu.SignExtendWord(value);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                if (size.Equals(Size.SizeLong))
                {
                    value = cpu.FetchPCLong();
                }
                else
                {
                    value = cpu.FetchPCWord();
                    if (size.Equals(Size.Byte))
                    {
                        value &= 0x00ff;
                    }
                }
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                throw new Exception("Cannot setByte on source only operand");
            }

            public virtual void SetLong(int value)
            {
                throw new Exception("Cannot setLong on source only operand");
            }

            public virtual void SetWord(int value)
            {
                throw new Exception("Cannot setWord on source only operand");
            }

            public override string ToString()
            {
                return $"#${value.ToString("X", CultureInfo.InvariantCulture)}";
            }
        }

        private class PCWithDisplacementOperand : IOperand
        {
            protected readonly int index = 9;

            protected int address;

            protected int displacement;

            protected Size size;

            private readonly ICPU cpu;

            public PCWithDisplacementOperand(ICPU icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.GetPC();
                displacement = cpu.FetchPCWordSigned();
                address += displacement;
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                return $"{displacement}(pc)";
            }
        }

        private class PCWithIndexOperand : IOperand
        {
            protected readonly int index = 10;

            protected int address;

            protected int displacement;

            protected bool idxIsAddressReg;

            protected int idxRegNumber;

            protected Size idxSize;

            protected Size size;

            private readonly CpuCore cpu;

            public PCWithIndexOperand(CpuCore icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public virtual int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public virtual int GetComputedAddress()
            {
                return address;
            }

            public virtual int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public virtual int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 14 : 10);
            }

            public virtual int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public virtual int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.GetPC();
                int ext = cpu.FetchPCWordSigned();
                displacement = cpu.SignExtendByte(ext);
                idxRegNumber = (ext >> 12) & 0x07;
                idxSize = ((ext & 0x0800) == 0x0800 ? Size.SizeLong : Size.Word);
                idxIsAddressReg = ((ext & 0x8000) == 0x8000);
                int idxVal;
                if (idxIsAddressReg)
                {
                    if (idxSize.Equals(Size.Word))
                    {
                        idxVal = cpu.GetAddrRegisterWordSigned(idxRegNumber);
                    }
                    else
                    {
                        idxVal = cpu.GetAddrRegisterLong(idxRegNumber);
                    }
                }
                else
                {
                    if (idxSize.Equals(Size.Word))
                    {
                        idxVal = cpu.GetDataRegisterWordSigned(idxRegNumber);
                    }
                    else
                    {
                        idxVal = cpu.GetDataRegisterLong(idxRegNumber);
                    }
                }

                address += displacement + idxVal;
            }

            public virtual bool IsRegisterMode()
            {
                return false;
            }

            public virtual bool IsSR()
            {
                return false;
            }

            public virtual void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public virtual void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public virtual void SetWord(int value)
            {
                cpu.WriteMemoryWord(address, value);
            }

            public override string ToString()
            {
                string appended;
                if (idxIsAddressReg)
                {
                    appended = "a";
                }
                else
                {
                    appended = "d";
                }
                return $"{displacement}(pc,{appended}{idxRegNumber}{idxSize.Ext})";
            }
        }

        private class StatusRegisterOperand : IOperand
        {
            protected readonly int index = 12;

            protected string name;

            protected Size size;

            protected int value;

            private readonly CpuCore cpu;

            public StatusRegisterOperand(CpuCore icpu)
            {
                cpu = icpu;
            }

            public virtual int GetByte()
            {
                return value & 0x00ff;
            }

            public virtual int GetByteSigned()
            {
                return cpu.SignExtendByte(value);
            }

            public virtual int GetComputedAddress()
            {
                throw new InvalidOperationException("Status Register has no computed address");
            }

            public virtual int GetLong()
            {
                throw new Exception("Cannot getLong on status register");
            }

            public virtual int GetTiming()
            {
                return 0;
            }

            public virtual int GetWord()
            {
                return value & 0x0000ffff;
            }

            public virtual int GetWordSigned()
            {
                return cpu.SignExtendWord(value);
            }

            public virtual int Index()
            {
                return index;
            }

            public virtual void Init(int param, Size sz)
            {
                size = sz;
                switch (size.Ext)
                {
                    case Size.BYTESIZE:
                        {
                            value = cpu.GetCCRegister();
                            name = "ccr";
                            break;
                        }

                    case Size.WORDSIZE:
                        {
                            value = cpu.GetSR();
                            name = "sr";
                            break;
                        }

                    default:
                        {
                            throw new ArgumentException("Status Register is byte or word access only");
                        }
                }
            }

            public virtual bool IsRegisterMode()
            {
                return true;
            }

            public virtual bool IsSR()
            {
                return true;
            }

            public virtual void SetByte(int value)
            {
                cpu.SetCCRegister(value);
            }

            public virtual void SetLong(int value)
            {
                throw new InvalidOperationException("Cannot setLong on status register");
            }

            public virtual void SetWord(int value)
            {
                cpu.SetSR(value);
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}