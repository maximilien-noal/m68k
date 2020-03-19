namespace M68k.CPU.Instructions
{
    public class NEGX : IInstructionHandler
    {
        private readonly ICPU cpu;

        public NEGX(ICPU cpu)
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
                    baseAddress = 0x4000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x4040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x4080;
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
            return new DisassembledInstruction(address, opcode, "negx" + sz.Ext, src);
        }

        protected virtual int NegxByte(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            int s = op.GetByte();
            int r = (0 - (s + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0)));
            op.SetByte(r);
            cpu.CalcFlags(InstructionType.NEGX, s, 0, r, Size.Byte);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected virtual int NegxLong(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int s = op.GetLong();
            int r = (0 - (s + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0)));
            op.SetLong(r);
            cpu.CalcFlags(InstructionType.NEGX, s, 0, r, Size.SizeLong);
            return (op.IsRegisterMode() ? 6 : 12 + op.GetTiming());
        }

        protected virtual int NegxWord(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = op.GetWord();
            int r = (0 - (s + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0)));
            op.SetWord(r);
            cpu.CalcFlags(InstructionType.NEGX, s, 0, r, Size.Word);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly NEGX parent;

            public AnonymousInstruction(NEGX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.NegxByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly NEGX parent;

            public AnonymousInstruction1(NEGX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.NegxWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly NEGX parent;

            public AnonymousInstruction2(NEGX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.NegxLong(opcode);
            }
        }
    }
}