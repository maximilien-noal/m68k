namespace M68k.CPU.Instructions
{
    public class MULU : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MULU(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0xc0c0;
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
            return new DisassembledInstruction(address, opcode, "mulu", src, dst);
        }

        protected int Mulu(int opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = op.GetWord();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetDataRegisterWord(reg);
            int r = s * d;
            if (r < 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            if (r == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            cpu.ClrFlags((cpu.VFlag | cpu.CFlag));
            cpu.SetDataRegisterLong(reg, r);
            int x = s;
            x = (x & 0x5555) + ((x >> 1) & 0x5555);
            x = (x & 0x3333) + ((x >> 2) & 0x3333);
            x = (x & 0x0f0f) + ((x >> 4) & 0x0f0f);
            x = (x & 0x00ff) + ((x >> 8) & 0x00ff);
            return 38 + (x << 1);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MULU parent;

            public AnonymousInstruction(MULU parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.Mulu(opcode);
            }
        }
    }
}