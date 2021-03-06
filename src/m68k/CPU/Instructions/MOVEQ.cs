using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class MOVEQ : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MOVEQ(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x7000;
            IInstruction i = new AnonymousInstruction(this);
            for (int reg = 0; reg < 8; reg++)
            {
                for (int imm = 0; imm < 256; imm++)
                {
                    instructionSet.AddInstruction(baseAddress + (reg << 9) + imm, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode)
        {
            DisassembledOperand src = new DisassembledOperand($"#${(opcode & 0xff).ToString("x2", CultureInfo.InvariantCulture)}");
            DisassembledOperand dst = new DisassembledOperand($"d{(opcode >> 9) & 0x07}");
            return new DisassembledInstruction(address, opcode, "moveq", src, dst);
        }

        protected int Moveq(int opcode)
        {
            int reg = (opcode >> 9 & 0x07);
            int data = CpuUtils.SignExtendWord(CpuUtils.SignExtendByte(opcode & 0xff));
            cpu.SetDataRegisterLong(reg, data);
            if (data < 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            if (data == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MOVEQ parent;

            public AnonymousInstruction(MOVEQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public int Execute(int opcode)
            {
                return parent.Moveq(opcode);
            }
        }
    }
}