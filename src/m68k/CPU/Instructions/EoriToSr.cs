using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class EoriToSr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public EoriToSr(ICPU cpu)
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
            baseAddress = 0x0a7c;
            i = new AnonymousInstruction(this);
            instructionSet.AddInstruction(baseAddress, i);
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int imm_bytes;
            int imm;
            string instructionSet;
            imm = cpu.ReadMemoryWord(address + 2);
            instructionSet = $"#${imm.ToString("x4", CultureInfo.InvariantCulture)}";
            imm_bytes = 2;
            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"EORI{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly EoriToSr parent;

            public AnonymousInstruction(EoriToSr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                int s = parent.cpu.FetchPCWordSigned();
                if (parent.cpu.IsSupervisorMode())
                {
                    s &= 0xf71f;
                    parent.cpu.SetSR(parent.cpu.GetSR() ^ s);
                }
                else
                {
                    parent.cpu.RaiseSRException();
                    return 34;
                }

                return 8;
            }
        }
    }
}