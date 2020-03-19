namespace M68k.CPU.Instructions
{
    public class RESET : IInstructionHandler
    {
        private readonly ICPU cpu;

        public RESET(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4e70, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly RESET parent;

            public AnonymousInstruction(RESET parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return new DisassembledInstruction(address, opcode, "reset");
            }

            public int Execute(int opcode)
            {
                if (parent.cpu.IsSupervisorMode())
                {
                    parent.cpu.ResetExternal();
                    return 132;
                }
                else
                {
                    parent.cpu.RaiseException(8);
                    return 34;
                }
            }
        }
    }
}