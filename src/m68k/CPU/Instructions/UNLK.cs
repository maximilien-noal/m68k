using System;

namespace M68k.CPU.Instructions
{
    public class UNLK : IInstructionHandler
    {
        private readonly ICPU cpu;

        public UNLK(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            DisassembledOperand src = new DisassembledOperand("a" + (opcode & 0x07));
            return new DisassembledInstruction(address, opcode, "unlk", src);
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress = 0x4e58;
            IInstruction i = new AnonymousInstruction(this);
            for (uint reg = 0; reg < 8; reg++)
            {
                instructionSet.AddInstruction(baseAddress + reg, i);
            }
        }

        protected virtual uint Unlk(uint opcode)
        {
            uint reg = (opcode & 0x007);
            cpu.SetAddrRegisterLong(7, cpu.GetAddrRegisterLong(reg));
            cpu.SetAddrRegisterLong(reg, cpu.PopLong());
            return 12;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly UNLK parent;

            public AnonymousInstruction(UNLK parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.Unlk(opcode);
            }
        }
    }
}