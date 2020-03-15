namespace M68k.CPU.Instructions
{
    public class TRAPV : IInstructionHandler
    {
        private readonly ICPU cpu;

        public TRAPV(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4e76, new AnonymousInstruction(this));
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            return new DisassembledInstruction(address, opcode, "trapv");
        }

        protected uint Trapv()
        {
            if (cpu.IsFlagSet(cpu.VFlag))
            {
                cpu.RaiseException(7);
                return 34;
            }

            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly TRAPV parent;

            public AnonymousInstruction(TRAPV parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.Trapv();
            }
        }
    }
}