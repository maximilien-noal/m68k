namespace M68k.CPU.Instructions
{
    public class RTS : IInstructionHandler
    {
        private readonly ICPU cpu;

        public RTS(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4e75, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly RTS parent;

            public AnonymousInstruction(RTS parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return new DisassembledInstruction(address, opcode, "rts");
            }

            public int Execute(int opcode)
            {
                parent.cpu.SetPC(parent.cpu.PopLong());
                return 16;
            }
        }
    }
}