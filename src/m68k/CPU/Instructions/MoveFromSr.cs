namespace M68k.CPU.Instructions
{
    public class MoveFromSr : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MoveFromSr(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress = 0x40c0;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("sr");
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }

        protected int DoMoveFromSr(int opcode)
        {
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            dst.GetWord();
            dst.SetWord(cpu.GetSR());
            return (dst.IsRegisterMode() ? 6 : 8 + dst.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MoveFromSr parent;

            public AnonymousInstruction(MoveFromSr parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.DoMoveFromSr(opcode);
            }
        }
    }
}