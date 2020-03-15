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
            public AnonymousInstruction(RESET parent)
            {
                this.parent = parent;
            }

            private readonly RESET parent;
            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return new DisassembledInstruction(address, opcode, "reset");
            }
        }
    }
}