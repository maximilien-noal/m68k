using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class AndiToSr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public AndiToSr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress;
            IInstruction i;
            baseAddress = 0x027c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected virtual int AndiWord(int opcode)
        {
            int s = cpu.FetchPCWordSigned();
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

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int imm_bytes;
            int imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = imm.ToString("x4", CultureInfo.InvariantCulture);
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"andi{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly AndiToSr parent;

            public AnonymousInstruction(AndiToSr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AndiWord(opcode);
            }
        }
    }
}