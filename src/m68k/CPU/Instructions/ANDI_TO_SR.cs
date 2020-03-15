using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class ANDI_TO_SR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public ANDI_TO_SR(ICPU cpu)
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
            baseAddress = 0x027c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(ANDI_TO_SR parent)
            {
                this.parent = parent;
            }

            private readonly ANDI_TO_SR parent;
            public uint Execute(uint opcode)
            {
                return parent.Andi_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected virtual uint Andi_word(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned();
            if (cpu.IsSupervisorMode())
            {
                s &= 0xf71f;
                cpu.SetSR(cpu.GetSR() & s);
            }
            else
            {
                cpu.RaiseSRException();
                return 34;
            }

            return 8;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint imm_bytes;
            uint imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = imm.ToString("x4", CultureInfo.InvariantCulture);
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"andi{sz.Ext}", src, dst);
        }
    }
}