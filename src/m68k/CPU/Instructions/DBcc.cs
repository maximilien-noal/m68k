using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class DBCC : IInstructionHandler
    {
        protected static readonly string[] names = new[] { "dbt", "dbra", "dbhi", "dbls", "dbcc", "dbcs", "dbne", "dbeq", "dbvc", "dbvs", "dbpl", "dbmi", "dbge", "dblt", "dbgt", "dble" };

        private readonly ICPU cpu;

        public DBCC(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress = 0x50c8;
            IInstruction i = new AnonymousInstruction(this);
            for (int cc = 0; cc < 16; cc++)
            {
                for (int r = 0; r < 8; r++)
                {
                    instructionSet.AddInstruction(baseAddress + (cc << 8) + r, i);
                }
            }
        }

        protected int Dbxx(int opcode)
        {
            int reg = (opcode & 0x07);
            int pc = cpu.GetPC();
            int dis = cpu.FetchPCWordSigned();
            int time;
            int count = cpu.GetDataRegisterWordSigned(reg) - 1;
            if (cpu.TestCC((opcode >> 8) & 0x0f))
            {
                time = 12;
            }
            else
            {
                cpu.SetDataRegisterWord(reg, count);
                if (count == -1)
                {
                    time = 14;
                }
                else
                {
                    cpu.SetPC(pc + dis);
                    time = 10;
                }
            }

            return time;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode)
        {
            int cc = (opcode >> 8) & 0x0f;
            int dis = cpu.ReadMemoryWordSigned(address + 2);
            DisassembledOperand reg = new DisassembledOperand($"d{opcode & 0x07}");
            DisassembledOperand where = new DisassembledOperand($"${(dis + address + 2).ToString("x8", CultureInfo.InvariantCulture)}", 2, dis);
            return new DisassembledInstruction(address, opcode, names[cc], reg, where);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly DBCC parent;

            public AnonymousInstruction(DBCC parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public int Execute(int opcode)
            {
                return parent.Dbxx(opcode);
            }
        }
    }
}