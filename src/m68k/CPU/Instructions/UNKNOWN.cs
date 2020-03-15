namespace M68k.CPU.Instructions
{
    public class UNKNOWN : IInstruction
    {
        private readonly ICPU cpu;
        public UNKNOWN(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual uint Execute(uint opcode)
        {
            uint vector;
            if ((opcode & 0xf000) == 0xa000)
            {
                vector = 10;
            }
            else if ((opcode & 0xf000) == 0xf000)
            {
                vector = 11;
            }
            else
            {
                vector = 4;
            }

            cpu.RaiseException(vector);
            return 34;
        }

        public virtual DisassembledInstruction Disassemble(uint address, uint opcode)
        {
            return new DisassembledInstruction(address, opcode, "????");
        }
    }
}