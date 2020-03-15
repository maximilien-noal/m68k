namespace M68k.CPU.Instructions
{
    public class RTR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public RTR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            instructionSet.AddInstruction(0x4e77, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly RTR parent;

            public AnonymousInstruction(RTR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return new DisassembledInstruction(address, opcode, "rtr");
            }

            public uint Execute(uint opcode)
            {
                parent.cpu.SetCCRegister(parent.cpu.PopWord());
                parent.cpu.SetPC(parent.cpu.PopLong());
                return 20;
            }
        }
    }
}