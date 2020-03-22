namespace M68k.CPU.Instructions
{
    public class ASL : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ASL(ICPU cpu)
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
                    baseAddress = 0xe100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe180;
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
                    baseAddress = 0xe120;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe160;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe1a0;
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

            baseAddress = 0xe1c0;
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

        protected int AslByteImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterByte(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
                msb = d & 0x80;
                if (msb != last_out)
                    msb_changed = 1;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.Byte);
            return 6 + shift + shift;
        }

        protected int AslByteReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterByte(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
                msb = d & 0x80;
                if (msb != last_out)
                    msb_changed = 1;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.Byte);
            return 6 + shift + shift;
        }

        protected int AslLongImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterLong(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = (int)(d & 0x80000000);
                d <<= 1;
                msb = (int)(d & 0x80000000);
                if (msb != last_out)
                {
                    msb_changed = 1;
                }
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int AslLongReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterLong(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = (int)(d & 0x80000000);
                d <<= 1;
                msb = (int)(d & 0x80000000);
                if (msb != last_out)
                {
                    msb_changed = 1;
                }
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected int AslWordImm(int opcode)
        {
            int shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterWord(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
                msb = d & 0x8000;
                if (msb != last_out)
                    msb_changed = 1;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.Word);
            return 6 + shift + shift;
        }

        protected int AslWordMem(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int v = op.GetWord();
            int last_out = v & 0x8000;
            int msb_changed = 0;
            v <<= 1;
            if ((v & 0x8000) != last_out)
            {
                msb_changed = 1;
            }

            op.SetWord(v);
            cpu.CalcFlagsParam(InstructionType.ASL, 1, last_out, v, msb_changed, Size.Word);
            return 8 + op.GetTiming();
        }

        protected int AslWordReg(int opcode)
        {
            int shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            int reg = (opcode & 0x07);
            int d = cpu.GetDataRegisterWord(reg);
            int msb;
            int last_out = 0;
            int msb_changed = 0;
            for (int s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
                msb = d & 0x8000;
                if (msb != last_out)
                    msb_changed = 1;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlagsParam(InstructionType.ASL, shift, last_out, d, msb_changed, Size.Byte);
            return 6 + shift + shift;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x00c0) == 0x00c0)
            {
                src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                return new DisassembledInstruction(address, opcode, "asl" + sz.Ext, src);
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

            return new DisassembledInstruction(address, opcode, "asl" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AslByteImm(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction1(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AslWordImm(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction2(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AslLongImm(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction3(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AslByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction4(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AslWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction5(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AslLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            private readonly ASL parent;

            public AnonymousInstruction6(ASL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AslWordMem(opcode);
            }
        }
    }
}