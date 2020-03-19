namespace M68k.CPU.Instructions
{
    public class NEG : IInstructionHandler
    {
        private readonly ICPU cpu;

        public NEG(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4400;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x4440;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x4480;
                    i = new AnonymousInstruction2(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "neg" + sz.Ext, src);
        }

        protected virtual int NegByte(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            int s = op.GetByte();
            int r = 0 - s;
            op.SetByte(r);
            cpu.CalcFlags(InstructionType.NEG, s, 0, r, Size.Byte);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected virtual int NegLong(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int s = op.GetLong();
            int r = 0 - s;
            op.SetLong(r);
            cpu.CalcFlags(InstructionType.NEG, s, 0, r, Size.SizeLong);
            return (op.IsRegisterMode() ? 6 : 12 + op.GetTiming());
        }

        protected virtual int NegWord(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = op.GetWord();
            int r = 0 - s;
            op.SetWord(r);
            cpu.CalcFlags(InstructionType.NEG, s, 0, r, Size.Word);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly NEG parent;

            public AnonymousInstruction(NEG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.NegByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly NEG parent;

            public AnonymousInstruction1(NEG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.NegWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly NEG parent;

            public AnonymousInstruction2(NEG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.NegLong(opcode);
            }
        }
    }
}