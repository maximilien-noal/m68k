namespace M68k.CPU.Instructions
{
    public class ROR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ROR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xe018;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe058;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe098;
                    i = new AnonymousInstruction2(this);
                }

                for (int imm = 0; imm < 8; imm++)
                {
                    for (int reg = 0; reg < 8; reg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (imm << 9) + reg, i);
                    }
                }
            }

            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xe038;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe078;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe0b8;
                    i = new AnonymousInstruction5(this);
                }

                for (int imm = 0; imm < 8; imm++)
                {
                    for (int reg = 0; reg < 8; reg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (imm << 9) + reg, i);
                    }
                }
            }

            baseAddress = 0xe6c0;
            i = new AnonymousInstruction6(this);
            for (int ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x00c0) == 0x00c0)
            {
                src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                return new DisassembledInstruction(address, opcode, "ror" + sz.Ext, src);
            }
            else if ((opcode & 0x0020) == 0x0020)
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                dst = new DisassembledOperand("d" + (opcode & 0x07));
            }
            else
            {
                int count = (opcode >> 9) & 0x07;
                if (count == 0)
                    count = 8;
                src = new DisassembledOperand("#" + count);
                dst = new DisassembledOperand("d" + (opcode & 0x07));
            }

            return new DisassembledInstruction(address, opcode, "ror" + sz.Ext, src, dst);
        }

        protected int RorByteImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterByte(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected int RorByteReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterByte(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected int RorLongImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterLong(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                {
                    d |= -2147483648;
                }
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int RorLongReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterLong(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                {
                    d |= -2147483648;
                }
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int RorWordImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterWord(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x8000;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected int RorWordMem(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int v = op.GetWord();
            int last_out = v & 0x01;
            v >>= 1;
            if (last_out != 0)
                v |= 0x8000;
            op.SetWord(v);
            cpu.CalcFlags(InstructionType.ROR, 1, last_out, v, Size.Word);
            return 8 + op.GetTiming();
        }

        protected int RorWordReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterWord(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x8000;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.RorByteImm(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction1(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.RorWordImm(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction2(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.RorLongImm(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction3(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.RorByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction4(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.RorWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction5(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.RorLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction6(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.RorWordMem(opcode);
            }
        }
    }
}