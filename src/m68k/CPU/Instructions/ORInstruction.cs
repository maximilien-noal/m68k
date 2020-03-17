namespace M68k.CPU.Instructions
{
    public class ORInstruction : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ORInstruction(ICPU cpu)
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
                    baseAddress = 0x8000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x8040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x8080;
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
                    baseAddress = 0x8100;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x8140;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0x8180;
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

            return new DisassembledInstruction(address, opcode, "or" + sz.Ext, src, dst);
        }

        protected uint OrByteDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint s = src.GetByteSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterByteSigned(reg);
            uint r = s | d;
            cpu.SetDataRegisterByte(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Byte);
            return time;
        }

        protected uint OrByteEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = s | d;
            dst.SetByte(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Byte);
            return time;
        }

        protected uint OrLongDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint s = src.GetLong();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterLong(reg);
            uint r = s | d;
            cpu.SetDataRegisterLong(reg, r);
            uint time = 6 + src.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.SizeLong);
            return time;
        }

        protected uint OrLongEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s | d;
            dst.SetLong(r);
            uint time = 12 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.SizeLong);
            return time;
        }

        protected uint OrWordDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint s = src.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterWordSigned(reg);
            uint r = s | d;
            cpu.SetDataRegisterWord(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Word);
            return time;
        }

        protected uint OrWordEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWordSigned();
            uint r = s | d;
            dst.SetWord(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Word);
            return time;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrByteDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction1(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrWordDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction2(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrLongDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction3(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrByteEaDest(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction4(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrWordEaDest(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ORInstruction parent;

            public AnonymousInstruction5(ORInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.OrLongEaDest(opcode);
            }
        }
    }
}