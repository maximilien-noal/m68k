namespace M68k.CPU.Instructions
{
    public class MoveToCcr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MoveToCcr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress = 0x44c0;
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
            DisassembledOperand dst = new DisassembledOperand("ccr");
            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }

        protected int DoMoveToCcr(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int s = src.GetWord() & 31;
            cpu.SetCCRegister(s);
            return 12 + src.GetTiming();
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MoveToCcr parent;

            public AnonymousInstruction(MoveToCcr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.DoMoveToCcr(opcode);
            }
        }
    }
}