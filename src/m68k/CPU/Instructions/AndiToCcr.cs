using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class AndiToCcr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public AndiToCcr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress;
            IInstruction i;
            baseAddress = 0x023c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected int AndiWord(int opcode)
        {
            int s = cpu.FetchPCWord() & 0x0031;
            cpu.SetSR(cpu.GetSR() & (s | 0xff00));
            return 8;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int imm_bytes;
            int imm;
            string instructionSet;
            switch (sz.Ext)
            {
                case Size.BYTESIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${(imm & 0x00ff).ToString("x2", CultureInfo.InvariantCulture)}";
                        imm_bytes = 2;
                        break;
                    }

                case Size.WORDSIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${imm.ToString("x4", CultureInfo.InvariantCulture)}";
                        imm_bytes = 2;
                        break;
                    }

                case Size.LONGSIZE:
                    {
                        imm = cpu.ReadMemoryLong(address + 2);
                        instructionSet = $"#${imm.ToString("x8", CultureInfo.InvariantCulture)}";
                        imm_bytes = 4;
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Size unsized for ANDI");
                    }
            }

            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"andi{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly AndiToCcr parent;

            public AnonymousInstruction(AndiToCcr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AndiWord(opcode);
            }
        }
    }
}