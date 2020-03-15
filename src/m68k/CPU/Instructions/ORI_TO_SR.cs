using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class ORI_TO_SR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ORI_TO_SR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            baseAddress = 0x007c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint imm_bytes;
            uint imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = imm.ToString("#$%04x", CultureInfo.InvariantCulture);
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "ori" + sz.Ext, src, dst);
        }

        protected virtual uint Ori_word(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned() & 0xf71f;
            if (cpu.IsSupervisorMode())
            {
                cpu.SetSR(cpu.GetSR() | s);
            }
            else
            {
                cpu.RaiseSRException();
                return 34;
            }

            return 8;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ORI_TO_SR parent;

            public AnonymousInstruction(ORI_TO_SR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Ori_word(opcode);
            }
        }
    }
}