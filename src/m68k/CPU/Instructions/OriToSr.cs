using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class OriToSr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public OriToSr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            baseAddress = 0x007c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int imm_bytes;
            int imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = imm.ToString("#$%04x", CultureInfo.InvariantCulture);
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "ori" + sz.Ext, src, dst);
        }

        protected int OriWord(int opcode)
        {
            int s = cpu.FetchPCWordSigned() & 0xf71f;
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
            private readonly OriToSr parent;

            public AnonymousInstruction(OriToSr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.OriWord(opcode);
            }
        }
    }
}