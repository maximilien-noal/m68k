using System;

namespace M68k.CPU
{
    public class DisassembledOperand
    {
        private readonly uint bytes;

        private readonly uint memRead;

        private readonly string operand;

        public DisassembledOperand(string operand)
        {
            this.operand = operand;
            bytes = 0;
            memRead = 0;
        }

        public DisassembledOperand(string operand, uint bytes, uint memRead)
        {
            this.operand = operand;
            this.bytes = bytes;
            this.memRead = memRead;
            if (bytes > 8)
                throw new ArgumentException("Are these the wrong way around ?");
        }

        public uint Bytes => bytes;

        public uint MemoryRead => memRead;

        public string Operand => operand;
    }
}