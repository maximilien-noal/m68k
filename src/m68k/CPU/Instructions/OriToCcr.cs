using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class OriToCcr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public OriToCcr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress;
            IInstruction i;
            baseAddress = 0x003c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint imm_bytes;
            uint imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = $"#${imm.ToString("x8", CultureInfo.InvariantCulture)}";
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "ori" + sz.Ext, src, dst);
        }

        protected virtual uint OriWord(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned() & 31;
            cpu.SetSR(cpu.GetSR() | s);
            return 8;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly OriToCcr parent;

            public AnonymousInstruction(OriToCcr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.OriWord(opcode);
            }
        }
    }
}