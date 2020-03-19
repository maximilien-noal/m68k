namespace M68k.CPU.Instructions
{
    public class SWAP : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SWAP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public DisassembledInstruction DisassembleOp(int address, int opcode)
        {
            DisassembledOperand src = new DisassembledOperand("d" + (opcode & 0x07));
            return new DisassembledInstruction(address, opcode, "swap", src);
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4840;
            IInstruction i = new AnonymousInstruction(this);
            for (int reg = 0; reg < 8; reg++)
            {
                instructionSet.AddInstruction(baseAddress + reg, i);
            }
        }

        protected virtual int Swap(int opcode)
        {
            int reg = (opcode & 0x007);
            int v = cpu.GetDataRegisterLong(reg);
            int vh = (v >> 16) & 0x0000ffff;
            v = (v << 16) + vh;
            cpu.SetDataRegisterLong(reg, v);
            cpu.CalcFlags(InstructionType.SWAP, v, v, v, Size.SizeLong);
            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SWAP parent;

            public AnonymousInstruction(SWAP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public int Execute(int opcode)
            {
                return parent.Swap(opcode);
            }
        }
    }
}