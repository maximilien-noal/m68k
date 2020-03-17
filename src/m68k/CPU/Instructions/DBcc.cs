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

        public virtual void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress = 0x50c8;
            IInstruction i = new AnonymousInstruction(this);
            for (uint cc = 0; cc < 16; cc++)
            {
                for (uint r = 0; r < 8; r++)
                {
                    instructionSet.AddInstruction(baseAddress + (cc << 8) + r, i);
                }
            }
        }

        protected uint Dbxx(uint opcode)
        {
            uint reg = (opcode & 0x07);
            uint pc = cpu.GetPC();
            uint dis = cpu.FetchPCWordSigned();
            uint time;
            int count = (int)cpu.GetDataRegisterWordSigned(reg) - 1;
            if (cpu.TestCC((opcode >> 8) & 0x0f))
            {
                time = 12;
            }
            else
            {
                cpu.SetDataRegisterWord(reg, (uint)count);
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            uint cc = (opcode >> 8) & 0x0f;
            uint dis = cpu.ReadMemoryWordSigned(address + 2);
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.Dbxx(opcode);
            }
        }
    }
}