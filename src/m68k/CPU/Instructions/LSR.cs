namespace M68k.CPU.Instructions
{
    public class LSR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public LSR(ICPU cpu)
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
                    baseAddress = 0xe008;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe048;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe088;
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
                    baseAddress = 0xe028;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe068;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe0a8;
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

            baseAddress = 0xe2c0;
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
                return new DisassembledInstruction(address, opcode, "lsr" + sz.Ext, src);
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

            return new DisassembledInstruction(address, opcode, "lsr" + sz.Ext, src, dst);
        }

        protected int LsrByteImm(int opcode)
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
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected int LsrByteReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterByte(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected int LsrLongImm(int opcode)
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
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int LsrLongReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterLong(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int LsrWordImm(int opcode)
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
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected int LsrWordMem(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int v = op.GetWord();
            int last_out = v & 0x01;
            v >>= 1;
            op.SetWord(v);
            cpu.CalcFlags(InstructionType.LSR, 1, last_out, v, Size.Word);
            return 8 + op.GetTiming();
        }

        protected int LsrWordReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterWord(reg);
            int last_out = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.LSR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.LsrByteImm(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction1(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.LsrWordImm(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction2(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.LsrLongImm(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction3(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.LsrByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction4(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.LsrWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction5(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.LsrLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            private readonly LSR parent;

            public AnonymousInstruction6(LSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.LsrWordMem(opcode);
            }
        }
    }
}