namespace M68k.CPU.Instructions
{
    public class RTE : IInstructionHandler
    {
        private readonly ICPU cpu;

        public RTE(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4e73, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly RTE parent;

            public AnonymousInstruction(RTE parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return new DisassembledInstruction(address, opcode, "rte");
            }

            public int Execute(int opcode)
            {
                if (parent.cpu.IsSupervisorMode())
                {
                    int newsr = parent.cpu.PopWord();
                    parent.cpu.SetPC(parent.cpu.PopLong());
                    parent.cpu.SetSR2(newsr);
                    return 20;
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