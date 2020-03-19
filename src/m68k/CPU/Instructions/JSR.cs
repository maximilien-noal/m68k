namespace M68k.CPU.Instructions
{
    public class JSR : IInstructionHandler
    {
        protected static readonly int[] TIMING = new int[] { 0, 0, 16, 0, 0, 18, 22, 18, 20, 18, 22 };

        private readonly ICPU cpu;

        public JSR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4e80;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 3 || ea_mode == 4)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 3)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "jsr", op);
        }

        protected int Jsr(int opcode)
        {
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.SizeLong);
            cpu.PushLong(cpu.GetPC());
            cpu.SetPC(op.GetComputedAddress());
            int idx;
            if (mode == 7)
                idx = mode + reg;
            else
                idx = mode;
            return TIMING[idx];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly JSR parent;

            public AnonymousInstruction(JSR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.Jsr(opcode);
            }
        }
    }
}