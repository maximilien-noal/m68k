namespace M68k.CPU.Instructions
{
    public class NOP : IInstructionHandler
    {
        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            instructionSet.AddInstruction(0x4e71, new AnonymousInstruction());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return new DisassembledInstruction(address, opcode, "nop");
            }

            public int Execute(int opcode)
            {
                return 4;
            }
        }
    }
}