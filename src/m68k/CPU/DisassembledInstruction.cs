using System.Globalization;
using System.Text;

namespace M68k.CPU
{
    public class DisassembledInstruction
    {
        private readonly uint address;

        private readonly string instruction;

        private readonly uint num_operands;

        private readonly DisassembledOperand op1;

        private readonly DisassembledOperand op2;

        private readonly uint opcode;

        public DisassembledInstruction(uint address, uint opcode, string instruction)
        {
            this.address = address;
            this.opcode = opcode;
            this.instruction = instruction;
            num_operands = 0;
            op1 = null;
            op2 = null;
        }

        public DisassembledInstruction(uint address, uint opcode, string instruction, DisassembledOperand dop)
        {
            this.address = address;
            this.opcode = opcode;
            this.instruction = instruction;
            num_operands = 1;
            op1 = dop;
            op2 = null;
        }

        public DisassembledInstruction(uint address, uint opcode, string instruction, DisassembledOperand dop1, DisassembledOperand dop2)
        {
            this.address = address;
            this.opcode = opcode;
            this.instruction = instruction;
            num_operands = 2;
            op1 = dop1;
            op2 = dop2;
        }

        public virtual void FormatInstruction(StringBuilder buffer)
        {
            if (buffer is null)
            {
                throw new System.ArgumentNullException(nameof(buffer));
            }

            buffer.Append($"{address.ToString("x8", CultureInfo.InvariantCulture)}   {opcode.ToString("x4", CultureInfo.InvariantCulture)}");
            switch (num_operands)
            {
                case 0:
                    {
                        buffer.Append("                    ").Append(instruction);
                        break;
                    }

                case 1:
                    {
                        if (op1.Bytes == 2)
                        {
                            buffer.Append($" {op1.MemoryRead.ToString("x4", CultureInfo.InvariantCulture)}               ");
                        }
                        else if (op1.Bytes == 4)
                        {
                            buffer.Append($" {op1.MemoryRead.ToString("x8", CultureInfo.InvariantCulture)}           ");
                        }
                        else
                        {
                            buffer.Append("                    ");
                        }

                        var ilen = instruction.Length;
                        buffer.Append(instruction);
                        while (ilen < 9)
                        {
                            buffer.Append(" ");
                            ilen++;
                        }

                        buffer.Append(op1.Operand);
                        break;
                    }

                case 2:
                    {
                        uint len = 0;
                        if (op1.Bytes == 2)
                        {
                            buffer.Append($" {op1.MemoryRead.ToString("x4", CultureInfo.InvariantCulture)}");
                            len += 5;
                        }
                        else if (op1.Bytes == 4)
                        {
                            buffer.Append($" {op1.MemoryRead.ToString("x8", CultureInfo.InvariantCulture)}");
                            len += 9;
                        }

                        if (op2.Bytes == 2)
                        {
                            buffer.Append($" {op2.MemoryRead.ToString("x4", CultureInfo.InvariantCulture)}");
                            len += 5;
                        }
                        else if (op2.Bytes == 4)
                        {
                            buffer.Append($" {op2.MemoryRead.ToString("x8", CultureInfo.InvariantCulture)}");
                            len += 9;
                        }

                        while (len < 21)
                        {
                            buffer.Append(" ");
                            len++;
                        }

                        var ilen = instruction.Length;
                        buffer.Append(instruction);
                        while (ilen < 9)
                        {
                            buffer.Append(" ");
                            ilen++;
                        }

                        buffer.Append(op1.Operand).Append(",").Append(op2.Operand);
                        break;
                    }
            }
        }

        public virtual void ShortFormat(StringBuilder buffer)
        {
            if (buffer is null)
            {
                throw new System.ArgumentNullException(nameof(buffer));
            }

            buffer.Append($"{address.ToString("x8", CultureInfo.InvariantCulture)}   ");
            switch (num_operands)
            {
                case 0:
                    {
                        buffer.Append(instruction);
                        break;
                    }

                case 1:
                    {
                        var ilen = instruction.Length;
                        buffer.Append(instruction);
                        while (ilen < 9)
                        {
                            buffer.Append(" ");
                            ilen++;
                        }

                        buffer.Append(op1.Operand);
                        break;
                    }

                case 2:
                    {
                        var ilen = instruction.Length;
                        buffer.Append(instruction);
                        while (ilen < 9)
                        {
                            buffer.Append(" ");
                            ilen++;
                        }

                        buffer.Append(op1.Operand).Append(",").Append(op2.Operand);
                        break;
                    }
            }
        }

        public virtual uint Size()
        {
            uint size = 2;
            if (num_operands == 2)
            {
                size += op1.Bytes + op2.Bytes;
            }
            else if (num_operands == 1)
            {
                size += op1.Bytes;
            }

            return size;
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder(80);
            FormatInstruction(buffer);
            return buffer.ToString();
        }
    }
}