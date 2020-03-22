using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class LINK : IInstructionHandler
    {
        private readonly ICPU cpu;

        public LINK(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public DisassembledInstruction DisassembleOp(int address, int opcode)
        {
            DisassembledOperand src = new DisassembledOperand("a" + (opcode & 0x07));
            int dis = cpu.ReadMemoryWordSigned(address + 2);
            DisassembledOperand dst = new DisassembledOperand($"#${dis.ToString("x4", CultureInfo.InvariantCulture)}", 2, dis);
            return new DisassembledInstruction(address, opcode, "link", src, dst);
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4e50;
            IInstruction i = new AnonymousInstruction(this);
            for (int reg = 0; reg < 8; reg++)
            {
                instructionSet.AddInstruction(baseAddress + reg, i);
            }
        }

        protected int Link(int opcode)
        {
            int sreg = (opcode & 0x007);
            int displacement = cpu.FetchPCWordSigned();
            cpu.PushLong(cpu.GetAddrRegisterLong(sreg));
            int sp = cpu.GetAddrRegisterLong(7);
            cpu.SetAddrRegisterLong(sreg, sp);
            cpu.SetAddrRegisterLong(7, sp + displacement);
            return 16;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly LINK parent;

            public AnonymousInstruction(LINK parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public int Execute(int opcode)
            {
                return parent.Link(opcode);
            }
        }
    }
}