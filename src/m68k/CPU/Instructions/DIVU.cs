namespace M68k.CPU.Instructions
{
    public class DIVU : IInstructionHandler
    {
        private readonly ICPU cpu;

        public DIVU(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress = 0x80c0;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 4)
                        break;
                    for (int r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "divu", src, dst);
        }

        protected int Divu(int opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = op.GetWord();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetDataRegisterLong(reg);
            int time;
            if (s == 0)
            {
                cpu.RaiseException(5);
                time = 38 + op.GetTiming();
            }
            else
            {
                long dl = d & 4294967295L;
                long quot = dl / (long)s;
                if (quot > 65535L)
                {
                    cpu.SetFlags(cpu.VFlag);
                    cpu.SetFlags(cpu.NFlag);
                }
                else
                {
                    int remain = (int)((dl % s) & 0xffff);
                    int result = (int)(quot & 0x0000ffff) | (remain << 16);
                    cpu.SetDataRegisterLong(reg, result);
                    if ((quot & 0x8000) != 0)
                    {
                        cpu.SetFlags(cpu.NFlag);
                    }
                    else
                    {
                        cpu.ClrFlags(cpu.NFlag);
                    }

                    if (quot == 0)
                    {
                        cpu.SetFlags(cpu.ZFlag);
                    }
                    else
                    {
                        cpu.ClrFlags(cpu.ZFlag);
                    }

                    cpu.ClrFlags((cpu.VFlag | cpu.CFlag));
                }

                time = 140 + op.GetTiming();
            }

            return time;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly DIVU parent;

            public AnonymousInstruction(DIVU parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.Divu(opcode);
            }
        }
    }
}