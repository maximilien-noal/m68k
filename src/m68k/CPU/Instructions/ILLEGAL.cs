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
            public AnonymousInstruction(ILLEGAL parent)
            {
                this.parent = parent;
            }

            private readonly ILLEGAL parent;
            public uint Execute(uint opcode)
            {
                parent.cpu.RaiseException(4);
                return 34;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return new DisassembledInstruction(address, opcode, "illegal");
            }
        }
    }
}