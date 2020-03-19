using System;

namespace M68k.CPU
{
    public class DisassembledOperand
    {
        private readonly int bytes;

        private readonly int memRead;

        private readonly string operand;

        public DisassembledOperand(string operand)
        {
            this.operand = operand;
            bytes = 0;
            memRead = 0;
        }

        public DisassembledOperand(string operand, int bytes, int memRead)
        {
            this.operand = operand;
            this.bytes = bytes;
            this.memRead = memRead;
            if (bytes > 8)
                throw new ArgumentException("Are these the wrong way around ?");
        }

        public int Bytes => bytes;

        public int MemoryRead => memRead;

        public string Operand => operand;
    }
}