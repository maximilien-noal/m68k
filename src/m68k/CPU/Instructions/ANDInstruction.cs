namespace M68k.CPU.Instructions
{
    public class ANDInstruction : IInstructionHandler
    {
        private readonly ICPU cpu;
        public ANDInstruction(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xc000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xc040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xc080;
                    i = new AnonymousInstruction2(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        if (ea_mode == 1)
                            continue;
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 4)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }

            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xc100;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xc140;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xc180;
                    i = new AnonymousInstruction5(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
                    {
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 1)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_byte_dn_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_word_dn_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_long_dn_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_byte_ea_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            public AnonymousInstruction4(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_word_ea_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            public AnonymousInstruction5(ANDInstruction parent)
            {
                this.parent = parent;
            }

            private readonly ANDInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.And_long_ea_dest(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint And_byte_dn_dest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint s = src.GetByteSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterByteSigned(reg);
            uint r = s & d;
            cpu.SetDataRegisterByte(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Byte);
            return time;
        }

        protected uint And_byte_ea_dest(uint opcode)
        {
            uint s = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = s & d;
            dst.SetByte(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Byte);
            return time;
        }

        protected uint And_word_dn_dest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint s = src.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterWordSigned(reg);
            uint r = s & d;
            cpu.SetDataRegisterWord(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Word);
            return time;
        }

        protected uint And_word_ea_dest(uint opcode)
        {
            uint s = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWordSigned();
            uint r = s & d;
            dst.SetWord(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Word);
            return time;
        }

        protected uint And_long_dn_dest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint s = src.GetLong();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterLong(reg);
            uint r = s & d;
            cpu.SetDataRegisterLong(reg, r);
            uint time = 6 + src.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.SizeLong);
            return time;
        }

        protected uint And_long_ea_dest(uint opcode)
        {
            uint s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s & d;
            dst.SetLong(r);
            uint time = 12 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.SizeLong);
            return time;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x0100) == 0)
            {
                src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            }

            return new DisassembledInstruction(address, opcode, "and" + sz.Ext, src, dst);
        }
    }
}