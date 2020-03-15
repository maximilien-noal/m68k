namespace M68k.CPU.Instructions
{
    public class SWAP : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SWAP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            DisassembledOperand src = new DisassembledOperand("d" + (opcode & 0x07));
            return new DisassembledInstruction(address, opcode, "swap", src);
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x4840;
            IInstruction i = new AnonymousInstruction(this);
            for (uint reg = 0; reg < 8; reg++)
            {
                instructionSet.AddInstruction(baseAddress + reg, i);
            }
        }

        protected virtual uint Swap(uint opcode)
        {
            uint reg = (opcode & 0x007);
            uint v = cpu.GetDataRegisterLong(reg);
            uint vh = (v >> 16) & 0x0000ffff;
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.Swap(opcode);
            }
        }
    }
}