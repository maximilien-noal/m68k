[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("test")]

namespace M68k.CPU
{
    using M68k.Memory;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public abstract class CpuCore : ICPU
    {
        internal const int C_FLAG = 1;

        internal const int N_FLAG = 8;

        internal const int V_FLAG = 2;

        internal const int X_FLAG = 16;

        internal const int Z_FLAG = 4;

        private const int INTERRUPT_FLAGS_MASK = 0x0700;

        private const int SUPERVISOR_FLAG = 0x2000;

        private const int TRACE_FLAG = 0x8000;

        private IOperand[] dstHandlers;

        private int reg_sr;

        private int reg_ssp;

        private int reg_usp;

        private IOperand[] srcHandlers;

        public IAddressSpace AddressSpace { get; set; }

        public List<int> AdressRegisters { get; } = new int[8].ToList();

        public int CFlag => C_FLAG;

        public int CurrentInstructionAddress { get; set; }

        public List<int> DataRegisters { get; } = new int[8].ToList();

        public StringBuilder DisasmBuffer { get; set; }

        public IOperand DstEAHandler { get; set; }

        public int InterruptFlagMask => INTERRUPT_FLAGS_MASK;

        public int NFlag => N_FLAG;

        public int RegPc { get; set; }

        public IOperand SrcEAHandler { get; set; }

        public int SupervisorFlag => SUPERVISOR_FLAG;

        public int TraceFlag => TRACE_FLAG;

        public int VFlag => V_FLAG;

        public int XFlag => X_FLAG;

        public int ZFlag => Z_FLAG;

        public void CalcFlags(InstructionType type, int src, int dst, int result, Size sz)
        {
            CalcFlagsParam(type, src, dst, result, 0, sz);
        }

        public void CalcFlagsParam(InstructionType type, int src, int dst, int result, int extraParam, Size sz)
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

        public void ClrFlags(int flags)
        {
            reg_sr &= ~(flags & 0x00ff);
        }

        public void DecrementAddrRegister(int reg, int numBytes)
        {
            AdressRegisters[reg] -= numBytes;
        }

        public DisassembledOperand DisassembleDstEA(int address, int mode, int reg, Size sz)
        {
            return DisassembleEA(address, mode, reg, sz, false);
        }

        public DisassembledOperand DisassembleSrcEA(int address, int mode, int reg, Size sz)
        {
            return DisassembleEA(address, mode, reg, sz, true);
        }

        public virtual int Execute()
        {
            throw new NotImplementedException($"Use {nameof(MC68000)}, not {nameof(CpuCore)}");
        }

        public int FetchPCLong()
        {
            int value = ReadMemoryLong(RegPc);
            RegPc += 4;
            return value;
        }

        public int FetchPCWord()
        {
            int value = ReadMemoryWord(RegPc);
            RegPc += 2;
            return value;
        }

        public int FetchPCWordSigned()
        {
            int value = ReadMemoryWordSigned(RegPc);
            RegPc += 2;
            return value;
        }

        public int GetAddrRegisterByte(int reg)
        {
            return AdressRegisters[reg] & 0x00ff;
        }

        public int GetAddrRegisterByteSigned(int reg)
        {
            return SignExtendByte(AdressRegisters[reg]);
        }

        public int GetAddrRegisterLong(int reg)
        {
            return AdressRegisters[reg];
        }

        public int GetAddrRegisterWord(int reg)
        {
            return AdressRegisters[reg] & 0x0000ffff;
        }

        public int GetAddrRegisterWordSigned(int reg)
        {
            return SignExtendWord(AdressRegisters[reg]);
        }

        public int GetCCRegister()
        {
            return reg_sr & 0x00ff;
        }

        public int GetDataRegisterByte(int reg)
        {
            return DataRegisters[reg] & 0x00ff;
        }

        public int GetDataRegisterByteSigned(int reg)
        {
            return SignExtendByte(DataRegisters[reg]);
        }

        public int GetDataRegisterLong(int reg)
        {
            return DataRegisters[reg];
        }

        public int GetDataRegisterWord(int reg)
        {
            return DataRegisters[reg] & 0x0000ffff;
        }

        public int GetDataRegisterWordSigned(int reg)
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

        public int GetInterruptLevel()
        {
            return (reg_sr >> 8) & 0x07;
        }

        public int GetPC()
        {
            return RegPc;
        }

        public int GetSR()
        {
            return reg_sr;
        }

        public int GetSSP()
        {
            return reg_ssp;
        }

        public int GetUSP()
        {
            return reg_usp;
        }

        public void IncrementAddrRegister(int reg, int numBytes)
        {
            AdressRegisters[reg] += numBytes;
        }

        public bool IsFlagSet(int flag)
        {
            return ((reg_sr & flag) == flag);
        }

        public bool IsSupervisorMode()
        {
            return (reg_sr & SUPERVISOR_FLAG) == SUPERVISOR_FLAG;
        }

        public int PopLong()
        {
            int val = ReadMemoryLong(AdressRegisters[7]);
            AdressRegisters[7] += 4;
            return val;
        }

        public int PopWord()
        {
            int val = ReadMemoryWord(AdressRegisters[7]);
            AdressRegisters[7] += 2;
            return val;
        }

        public void PushLong(int value)
        {
            AdressRegisters[7] -= 4;
            WriteMemoryLong(AdressRegisters[7], value);
        }

        public void PushWord(int value)
        {
            AdressRegisters[7] -= 2;
            WriteMemoryWord(AdressRegisters[7], value);
        }

        public void RaiseException(int vector)
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

        public void RaiseInterrupt(int priority)
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

        public void RaiseSRException()
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

        public int ReadMemoryByte(int addr)
        {
            return AddressSpace.ReadByte(addr);
        }

        public int ReadMemoryByteSigned(int addr)
        {
            return SignExtendByte(AddressSpace.ReadByte(addr));
        }

        public int ReadMemoryLong(int addr)
        {
            return (int)AddressSpace.ReadLong(addr);
        }

        public int ReadMemoryWord(int addr)
        {
            return AddressSpace.ReadWord(addr);
        }

        public int ReadMemoryWordSigned(int addr)
        {
            return SignExtendWord(AddressSpace.ReadWord(addr));
        }

        public void Reset()
        {
            reg_ssp = (int)AddressSpace.ReadLong(0);
            AdressRegisters[7] = reg_ssp;

            RegPc = (int)AddressSpace.ReadLong(4);
            reg_sr = 0x2700;
        }

        public void ResetExternal()
        {
            // Method intentionally left empty.
        }

        public IOperand ResolveDstEA(int mode, int reg, Size size)
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

        public IOperand ResolveSrcEA(int mode, int reg, Size size)
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

        public void SetAddressSpace(IAddressSpace memory)
        {
            this.AddressSpace = memory;
        }

        public void SetAddrRegisterByte(int reg, int value)
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

        public void SetAddrRegisterLong(int reg, int value)
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

        public void SetAddrRegisterWord(int reg, int value)
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

        public void SetCCRegister(int value)
        {
            reg_sr = (reg_sr & 0xff00) | (value & 0x00ff);
        }

        public void SetDataRegisterByte(int reg, int value)
        {
            DataRegisters[reg] = (int)(DataRegisters[reg] & 0xffffff00) | (value & 0x00ff);
        }

        public void SetDataRegisterLong(int reg, int value)
        {
            DataRegisters[reg] = value;
        }

        public void SetDataRegisterWord(int reg, int value)
        {
            DataRegisters[reg] = (int)(DataRegisters[reg] & 0xffff0000) | (value & 0x0000ffff);
        }

        public void SetFlags(int flags)
        {
            reg_sr |= (flags & 0x00ff);
        }

        public void SetPC(int address)
        {
            RegPc = address;
        }

        public void SetSR(int value)
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

        public void SetSR2(int value)
        {
            reg_sr = value;
            if ((reg_sr & SUPERVISOR_FLAG) == 0)
            {
                reg_ssp = AdressRegisters[7];
                AdressRegisters[7] = reg_usp;
            }
        }

        public void SetSSP(int address)
        {
            reg_ssp = address;
            if (IsSupervisorMode())
                AdressRegisters[7] = reg_ssp;
        }

        public void SetSupervisorMode(bool enable)
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

        public void SetUSP(int address)
        {
            reg_usp = address;
            if (!IsSupervisorMode())
            {
                AdressRegisters[7] = reg_usp;
            }
        }

        public void StopNow()
        {
        }

        public bool TestCC(int cc)
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

        public void WriteMemoryByte(int addr, int value)
        {
            AddressSpace.WriteByte(addr, value);
        }

        public void WriteMemoryLong(int addr, int value)
        {
            AddressSpace.WriteLong(addr, (uint)value);
        }

        public void WriteMemoryWord(int addr, int value)
        {
            AddressSpace.WriteWord(addr, value);
        }

        internal int SignExtendByte(int value)
        {
            if ((value & 0x80) == 0x80)
            {
                value |= -256;
            }
            else
            {
                value &= 0x000000ff;
            }

            return value;
        }

        internal int SignExtendWord(int value)
        {
            if ((value & 0x8000) == 0x8000)
            {
                value |= -65536;
            }
            else
            {
                value &= 0x0000ffff;
            }

            return value;
        }

        protected DisassembledOperand DisassembleEA(int address, int mode, int reg, Size sz, bool isSrc)
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

        protected void InitEAHandlers()
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

        protected void SetInterruptLevel(int level)
        {
            reg_sr &= ~(INTERRUPT_FLAGS_MASK);
            reg_sr |= (level & 0x07) << 8;
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 16 : 12);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.FetchPCLong();
            }

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.FetchPCWordSigned();
            }

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 8 : 4);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                address = cpu.GetAddrRegisterLong(regNumber);
            }

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.GetAddrRegisterByte(regNumber);
            }

            public int GetByteSigned()
            {
                return cpu.GetAddrRegisterByteSigned(regNumber);
            }

            public int GetComputedAddress()
            {
                throw new Exception("Address Register direct has no computed address");
            }

            public int GetLong()
            {
                return cpu.GetAddrRegisterLong(regNumber);
            }

            public int GetTiming()
            {
                return 0;
            }

            public int GetWord()
            {
                return cpu.GetAddrRegisterWord(regNumber);
            }

            public int GetWordSigned()
            {
                return cpu.GetAddrRegisterWordSigned(regNumber);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
            }

            public bool IsRegisterMode()
            {
                return true;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.SetAddrRegisterByte(regNumber, value);
            }

            public void SetLong(int value)
            {
                cpu.SetAddrRegisterLong(regNumber, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 8 : 4);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 10 : 6);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryWord(address + 2, value & 0xFFFF);
                cpu.WriteMemoryWord(address, (value >> 16) & 0xFFFF);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
                displacement = cpu.FetchPCWordSigned();
                address = cpu.GetAddrRegisterLong(regNumber) + displacement;
            }

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 14 : 10);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.GetDataRegisterByte(regNumber);
            }

            public int GetByteSigned()
            {
                return cpu.GetDataRegisterByteSigned(regNumber);
            }

            public int GetComputedAddress()
            {
                throw new Exception("Data Register has no computed address");
            }

            public int GetLong()
            {
                return cpu.GetDataRegisterLong(regNumber);
            }

            public int GetTiming()
            {
                return 0;
            }

            public int GetWord()
            {
                return cpu.GetDataRegisterWord(regNumber);
            }

            public int GetWordSigned()
            {
                return cpu.GetDataRegisterWordSigned(regNumber);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                regNumber = param;
                size = sz;
            }

            public bool IsRegisterMode()
            {
                return true;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.SetDataRegisterByte(regNumber, value);
            }

            public void SetLong(int value)
            {
                cpu.SetDataRegisterLong(regNumber, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return value & 0x00ff;
            }

            public int GetByteSigned()
            {
                return cpu.SignExtendByte(value);
            }

            public int GetComputedAddress()
            {
                throw new Exception("Immediate addressing has no computed address");
            }

            public int GetLong()
            {
                return value;
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public int GetWord()
            {
                return value & 0x0000ffff;
            }

            public int GetWordSigned()
            {
                return cpu.SignExtendWord(value);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                throw new Exception("Cannot setByte on source only operand");
            }

            public void SetLong(int value)
            {
                throw new Exception("Cannot setLong on source only operand");
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 12 : 8);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
            {
                size = sz;
                address = cpu.GetPC();
                displacement = cpu.FetchPCWordSigned();
                address += displacement;
            }

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return cpu.ReadMemoryByte(address);
            }

            public int GetByteSigned()
            {
                return cpu.ReadMemoryByteSigned(address);
            }

            public int GetComputedAddress()
            {
                return address;
            }

            public int GetLong()
            {
                return cpu.ReadMemoryLong(address);
            }

            public int GetTiming()
            {
                return (size.Equals(Size.SizeLong) ? 14 : 10);
            }

            public int GetWord()
            {
                return cpu.ReadMemoryWord(address);
            }

            public int GetWordSigned()
            {
                return cpu.ReadMemoryWordSigned(address);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return false;
            }

            public bool IsSR()
            {
                return false;
            }

            public void SetByte(int value)
            {
                cpu.WriteMemoryByte(address, value);
            }

            public void SetLong(int value)
            {
                cpu.WriteMemoryLong(address, value);
            }

            public void SetWord(int value)
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

            public int GetByte()
            {
                return value & 0x00ff;
            }

            public int GetByteSigned()
            {
                return cpu.SignExtendByte(value);
            }

            public int GetComputedAddress()
            {
                throw new InvalidOperationException("Status Register has no computed address");
            }

            public int GetLong()
            {
                throw new Exception("Cannot getLong on status register");
            }

            public int GetTiming()
            {
                return 0;
            }

            public int GetWord()
            {
                return value & 0x0000ffff;
            }

            public int GetWordSigned()
            {
                return cpu.SignExtendWord(value);
            }

            public int Index()
            {
                return index;
            }

            public void Init(int param, Size sz)
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

            public bool IsRegisterMode()
            {
                return true;
            }

            public bool IsSR()
            {
                return true;
            }

            public void SetByte(int value)
            {
                cpu.SetCCRegister(value);
            }

            public void SetLong(int value)
            {
                throw new InvalidOperationException("Cannot setLong on status register");
            }

            public void SetWord(int value)
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