namespace M68k.CPU.Instructions
{
    public class JMP : IInstructionHandler
    {
        protected static readonly int[] TIMING = new int[] { 0, 0, 8, 0, 0, 10, 14, 10, 12, 10, 14 };

        private readonly ICPU cpu;

        public JMP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4ec0;
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
            return new DisassembledInstruction(address, opcode, "jmp", op);
        }

        protected int Jmp(int opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            cpu.SetPC(op.GetComputedAddress());
            return TIMING[op.Index()];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly JMP parent;

            public AnonymousInstruction(JMP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.Jmp(opcode);
            }
        }
    }
}