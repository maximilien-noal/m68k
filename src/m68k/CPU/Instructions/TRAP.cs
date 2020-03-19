using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class TRAP : IInstructionHandler
    {
        private readonly ICPU cpu;

        public TRAP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress = 0x4e40;
            IInstruction i = new AnonymousInstruction(this);
            for (int v = 0; v < 16; v++)
            {
                instructionSet.AddInstruction(baseAddress + v, i);
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode)
        {
            DisassembledOperand op = new DisassembledOperand((opcode & 0x0f).ToString("d", CultureInfo.InvariantCulture));
            return new DisassembledInstruction(address, opcode, "trap", op);
        }

        protected int Trap(int opcode)
        {
            int v = (opcode & 0x0f);
            cpu.RaiseException(32 + v);
            return 34;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly TRAP parent;

            public AnonymousInstruction(TRAP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public int Execute(int opcode)
            {
                return parent.Trap(opcode);
            }
        }
    }
}