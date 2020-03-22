using System.Text;

namespace M68k.CPU.Instructions
{
    public class MOVEM : IInstructionHandler
    {
        private static readonly int[] M2R_Timing = new int[] { 0, 0, 12, 12, 0, 16, 18, 16, 20, 16, 18 };

        private static readonly int[] R2M_Timing = new int[] { 0, 0, 8, 0, 8, 12, 14, 12, 16 };

        private readonly ICPU cpu;

        public MOVEM(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public DisassembledInstruction DisassembleOp(int address, int opcode, Size sz, bool regToMem)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            int reglist = cpu.ReadMemoryWord(address + 2);
            bool reversed = (mode == 4);
            if (regToMem)
            {
                src = new DisassembledOperand(RegListToString(reglist, reversed), 2, reglist);
                dst = cpu.DisassembleDstEA(address + 4, mode, reg, sz);
            }
            else
            {
                src = cpu.DisassembleSrcEA(address + 4, mode, reg, sz);
                dst = new DisassembledOperand(RegListToString(reglist, reversed), 2, reglist);
            }

            return new DisassembledInstruction(address, opcode, "movem" + sz.Ext, src, dst);
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4880;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x48c0;
                    i = new AnonymousInstruction1(this);
                }

                for (int ea_mode = 2; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 3)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4c80;
                    i = new AnonymousInstruction2(this);
                }
                else
                {
                    baseAddress = 0x4cc0;
                    i = new AnonymousInstruction3(this);
                }

                for (int ea_mode = 2; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 4)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 3)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected int GetMultipleLong(int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetAddrRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            return regcount;
        }

        protected int GetMultipleLongPostInc(int reg, int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetAddrRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            cpu.SetAddrRegisterLong(reg, start);
            return regcount;
        }

        protected int GetMultipleWord(int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetAddrRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            return regcount;
        }

        protected int GetMultipleWordPostInc(int reg, int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetAddrRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            cpu.SetAddrRegisterLong(reg, start);
            return regcount;
        }

        protected int MovemLongM2r(int opcode)
        {
            int reglist = cpu.FetchPCWord();
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int address = src.GetComputedAddress();
            int count;
            if (((opcode >> 3) & 0x07) == 3)
            {
                count = GetMultipleLongPostInc(opcode & 0x07, reglist, address);
            }
            else
            {
                count = GetMultipleLong(reglist, address);
            }

            return M2R_Timing[src.Index()] + (count << 3);
        }

        protected int MovemLongR2m(int opcode)
        {
            int reglist = cpu.FetchPCWord();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int address = dst.GetComputedAddress();
            int count;
            if (((opcode >> 3) & 0x07) == 4)
            {
                count = PutMultipleLongPreDec(opcode & 0x07, reglist, address);
            }
            else
            {
                count = PutMultipleLong(reglist, address);
            }

            return R2M_Timing[dst.Index()] + (count << 3);
        }

        protected int MovemWordM2r(int opcode)
        {
            int reglist = cpu.FetchPCWord();
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int address = src.GetComputedAddress();
            int count;
            if (((opcode >> 3) & 0x07) == 3)
            {
                count = GetMultipleWordPostInc(opcode & 0x07, reglist, address);
            }
            else
            {
                count = GetMultipleWord(reglist, address);
            }

            return M2R_Timing[src.Index()] + (count << 2);
        }

        protected int MovemWordR2m(int opcode)
        {
            int reglist = cpu.FetchPCWord();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int address = dst.GetComputedAddress();
            int count;
            if (((opcode >> 3) & 0x07) == 4)
            {
                count = PutMultipleWordPreDec(opcode & 0x07, reglist, address);
            }
            else
            {
                count = PutMultipleWord(reglist, address);
            }

            return R2M_Timing[dst.Index()] + (count << 2);
        }

        protected int PutMultipleLong(int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryLong(start, cpu.GetDataRegisterLong(n));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryLong(start, cpu.GetAddrRegisterLong(n));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            return regcount;
        }

        protected int PutMultipleLongPreDec(int reg, int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address + 4;
            int oldreg = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    start -= 4;
                    if (reg == 7 - n)
                        WriteMemoryLongSwapped(start, oldreg);
                    else
                        WriteMemoryLongSwapped(start, cpu.GetAddrRegisterLong(7 - n));
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    start -= 4;
                    WriteMemoryLongSwapped(start, cpu.GetDataRegisterLong(7 - n));
                    regcount++;
                }

                bit <<= 1;
            }

            cpu.SetAddrRegisterLong(reg, start);
            return regcount;
        }

        protected int PutMultipleWord(int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryWord(start, cpu.GetDataRegisterWord(n));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryWord(start, cpu.GetAddrRegisterWord(n));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            return regcount;
        }

        protected int PutMultipleWordPreDec(int reg, int reglist, int address)
        {
            int bit = 1;
            int regcount = 0;
            int start = address + 2;
            int oldreg = address & 0xffff;
            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    start -= 2;
                    if (reg == 7 - n)
                        cpu.WriteMemoryWord(start, oldreg);
                    else
                        cpu.WriteMemoryWord(start, cpu.GetAddrRegisterWord(7 - n));
                    regcount++;
                }

                bit <<= 1;
            }

            for (int n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    start -= 2;
                    cpu.WriteMemoryWord(start, cpu.GetDataRegisterWord(7 - n));
                    regcount++;
                }

                bit <<= 1;
            }

            cpu.SetAddrRegisterLong(reg, start);
            return regcount;
        }

        protected string RegListToString(int reglist, bool reversed)
        {
            StringBuilder sb = new StringBuilder();
            int first = -1;
            int count = 0;
            if (!reversed)
            {
                char prefix = 'd';
                int mask = 1;
                for (int i = 0; i < 2; i++)
                {
                    for (int n = 0; n < 8; n++, mask <<= 1)
                    {
                        if ((reglist & mask) != 0)
                        {
                            if (first != -1)
                            {
                                count++;
                            }
                            else
                            {
                                first = n;
                            }
                        }
                        else
                        {
                            if (first != -1)
                            {
                                if (sb.Length > 0)
                                    sb.Append('/');
                                sb.Append(prefix);
                                sb.Append(first);
                                if (count == 1)
                                {
                                    sb.Append('/');
                                    sb.Append(prefix);
                                    sb.Append(n - 1);
                                }
                                else if (count > 1)
                                {
                                    sb.Append('-');
                                    sb.Append(prefix);
                                    sb.Append(n - 1);
                                }

                                count = 0;
                                first = -1;
                            }
                        }
                    }

                    if (first != -1)
                    {
                        if (sb.Length > 0)
                            sb.Append('/');
                        sb.Append(prefix);
                        sb.Append(first);
                        if (count == 1)
                        {
                            sb.Append('/');
                            sb.Append(prefix);
                            sb.Append(7);
                        }
                        else if (count > 1)
                        {
                            sb.Append('-');
                            sb.Append(prefix);
                            sb.Append(7);
                        }

                        count = 0;
                        first = -1;
                    }

                    prefix = 'a';
                }
            }
            else
            {
                char prefix = 'd';
                int mask = 0x8000;
                for (int i = 0; i < 2; i++)
                {
                    for (int n = 0; n < 8; n++, mask >>= 1)
                    {
                        if ((reglist & mask) != 0)
                        {
                            if (first != -1)
                            {
                                count++;
                            }
                            else
                            {
                                first = n;
                            }
                        }
                        else
                        {
                            if (first != -1)
                            {
                                if (sb.Length > 0)
                                    sb.Append('/');
                                sb.Append(prefix);
                                sb.Append(first);
                                if (count == 1)
                                {
                                    sb.Append('/');
                                    sb.Append(prefix);
                                    sb.Append(n - 1);
                                }
                                else if (count > 1)
                                {
                                    sb.Append('-');
                                    sb.Append(prefix);
                                    sb.Append(n - 1);
                                }

                                count = 0;
                                first = -1;
                            }
                        }
                    }

                    if (first != -1)
                    {
                        if (sb.Length > 0)
                            sb.Append('/');
                        sb.Append(prefix);
                        sb.Append(first);
                        if (count == 1)
                        {
                            sb.Append('/');
                            sb.Append(prefix);
                            sb.Append(7);
                        }
                        else if (count > 1)
                        {
                            sb.Append('-');
                            sb.Append(prefix);
                            sb.Append(7);
                        }

                        count = 0;
                        first = -1;
                    }

                    prefix = 'a';
                }
            }

            return sb.ToString();
        }

        private void WriteMemoryLongSwapped(int address, int value)
        {
            cpu.WriteMemoryWord(address + 2, value & 0xFFFF);
            cpu.WriteMemoryWord(address, (value >> 16) & 0xFFFF);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MOVEM parent;

            public AnonymousInstruction(MOVEM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, true);
            }

            public int Execute(int opcode)
            {
                return parent.MovemWordR2m(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly MOVEM parent;

            public AnonymousInstruction1(MOVEM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, true);
            }

            public int Execute(int opcode)
            {
                return parent.MovemLongR2m(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly MOVEM parent;

            public AnonymousInstruction2(MOVEM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, false);
            }

            public int Execute(int opcode)
            {
                return parent.MovemWordM2r(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly MOVEM parent;

            public AnonymousInstruction3(MOVEM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, false);
            }

            public int Execute(int opcode)
            {
                return parent.MovemLongM2r(opcode);
            }
        }
    }
}