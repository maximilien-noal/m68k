namespace M68k.CPU.Instructions
{
    public class ILLEGAL : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ILLEGAL(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4afc, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ILLEGAL parent;

            public AnonymousInstruction(ILLEGAL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return new DisassembledInstruction(address, opcode, "illegal");
            }

            public int Execute(int opcode)
            {
                parent.cpu.RaiseException(4);
                return 34;
            }
        }
    }
}