namespace M68k.CPU.Instructions
{
    public class MoveToSr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MoveToSr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x46c0;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 4)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("sr");
            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }

        protected int DoMoveToSr(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int s = src.GetWord() & 0xf71f;
            if (!cpu.IsSupervisorMode())
            {
                cpu.RaiseSRException();
                return 34;
            }

            cpu.SetSR(s);
            return 12 + src.GetTiming();
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MoveToSr parent;

            public AnonymousInstruction(MoveToSr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.DoMoveToSr(opcode);
            }
        }
    }
}