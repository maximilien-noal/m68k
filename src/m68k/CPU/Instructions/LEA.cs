namespace M68k.CPU.Instructions
{
    public class LEA : IInstructionHandler
    {
        protected static readonly int[] TIMING = new int[] { 0, 0, 4, 0, 0, 8, 12, 8, 12, 8, 12 };

        private readonly ICPU cpu;

        public LEA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x41c0;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 3 || ea_mode == 4)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 3)
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
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "lea", src, dst);
        }

        protected int Lea(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetComputedAddress());
            return TIMING[src.Index()];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly LEA parent;

            public AnonymousInstruction(LEA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.Lea(opcode);
            }
        }
    }
}