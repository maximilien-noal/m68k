using System.Text;

namespace M68k.CPU.Instructions
{
    public class MOVEM : IInstructionHandler
    {
        private readonly ICPU cpu;
        private static readonly uint[] M2R_Timing = new uint[]{0, 0, 12, 12, 0, 16, 18, 16, 20, 16, 18};
        private static readonly uint[] R2M_Timing = new uint[]{0, 0, 8, 0, 8, 12, 14, 12, 16};
        public MOVEM(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 2; sz++)
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

                for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 3)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            for (uint sz = 0; sz < 2; sz++)
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

                for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 4)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 3)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(MOVEM parent)
            {
                this.parent = parent;
            }

            private readonly MOVEM parent;
            public uint Execute(uint opcode)
            {
                return parent.Movem_word_r2m(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, true);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(MOVEM parent)
            {
                this.parent = parent;
            }

            private readonly MOVEM parent;
            public uint Execute(uint opcode)
            {
                return parent.Movem_long_r2m(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, true);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(MOVEM parent)
            {
                this.parent = parent;
            }

            private readonly MOVEM parent;
            public uint Execute(uint opcode)
            {
                return parent.Movem_word_m2r(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, false);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(MOVEM parent)
            {
                this.parent = parent;
            }

            private readonly MOVEM parent;
            public uint Execute(uint opcode)
            {
                return parent.Movem_long_m2r(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, false);
            }
        }

        protected uint Movem_word_r2m(uint opcode)
        {
            uint reglist = cpu.FetchPCWord();
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint address = dst.GetComputedAddress();
            uint count;
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

        protected uint Movem_long_r2m(uint opcode)
        {
            uint reglist = cpu.FetchPCWord();
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint address = dst.GetComputedAddress();
            uint count;
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

        protected uint Movem_word_m2r(uint opcode)
        {
            uint reglist = cpu.FetchPCWord();
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint address = src.GetComputedAddress();
            uint count;
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

        protected uint Movem_long_m2r(uint opcode)
        {
            uint reglist = cpu.FetchPCWord();
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint address = src.GetComputedAddress();
            uint count;
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

        public DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz, bool reg_to_mem)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            uint reglist = cpu.ReadMemoryWord(address + 2);
            bool reversed = (mode == 4);
            if (reg_to_mem)
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

        protected string RegListToString(uint reglist, bool reversed)
        {
            StringBuilder sb = new StringBuilder();
            int first = -1;
            uint count = 0;
            if (!reversed)
            {
                char prefix = 'd';
                uint mask = 1;
                for (uint i = 0; i < 2; i++)
                {
                    for (uint n = 0; n < 8; n++, mask <<= 1)
                    {
                        if ((reglist & mask) != 0)
                        {
                            if (first != -1)
                            {
                                count++;
                            }
                            else
                            {
                                first = (int)n;
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
                uint mask = 0x8000;
                for (uint i = 0; i < 2; i++)
                {
                    for (uint n = 0; n < 8; n++, mask >>= 1)
                    {
                        if ((reglist & mask) != 0)
                        {
                            if (first != -1)
                            {
                                count++;
                            }
                            else
                            {
                                first = (int)n;
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

        protected virtual uint GetMultipleWord(uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected uint GetMultipleWordPostInc(uint reg, uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryWordSigned(start));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected virtual uint GetMultipleLong(uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected uint GetMultipleLongPostInc(uint reg, uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.SetDataRegisterLong(n, cpu.ReadMemoryLong(start));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected uint PutMultipleWord(uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryWord(start, cpu.GetDataRegisterWord(n));
                    start += 2;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected uint PutMultipleWordPreDec(uint reg, uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address + 2;
            uint oldreg = address & 0xffff;
            for (uint n = 0; n < 8; n++)
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

            for (uint n = 0; n < 8; n++)
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

        protected uint PutMultipleLong(uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address;
            for (uint n = 0; n < 8; n++)
            {
                if ((reglist & bit) != 0)
                {
                    cpu.WriteMemoryLong(start, cpu.GetDataRegisterLong(n));
                    start += 4;
                    regcount++;
                }

                bit <<= 1;
            }

            for (uint n = 0; n < 8; n++)
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

        protected uint PutMultipleLongPreDec(uint reg, uint reglist, uint address)
        {
            uint bit = 1;
            uint regcount = 0;
            uint start = address + 4;
            uint oldreg = address;
            for (uint n = 0; n < 8; n++)
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

            for (uint n = 0; n < 8; n++)
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

        private void WriteMemoryLongSwapped(uint address, uint value)
        {
            cpu.WriteMemoryWord(address + 2, value & 0xFFFF);
            cpu.WriteMemoryWord(address, (value >> 16) & 0xFFFF);
        }
    }
}