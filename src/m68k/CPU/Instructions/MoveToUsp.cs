namespace M68k.CPU.Instructions
{
    public class MoveToUsp : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MoveToUsp(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress;
            IInstruction i;
            for (int t = 0; t < 2; t++)
            {
                if (t == 0)
                {
                    baseAddress = 0x4e60;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x4e68;
                    i = new AnonymousInstruction1(this);
                }

                for (int reg = 0; reg < 8; reg++)
                {
                    instructionSet.AddInstruction(baseAddress + reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, bool moveToUsp)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if (moveToUsp)
            {
                src = new DisassembledOperand("a" + (opcode & 0x07));
                dst = new DisassembledOperand("usp");
            }
            else
            {
                src = new DisassembledOperand("usp");
                dst = new DisassembledOperand("a" + (opcode & 0x07));
            }

            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }

        protected int DoMoveToUsp(int opcode)
        {
            if (!cpu.IsSupervisorMode())
            {
                cpu.RaiseSRException();
                return 34;
            }

            cpu.SetUSP(cpu.GetAddrRegisterLong(opcode & 0x07));
            return 4;
        }

        protected int MoveFromUsp(int opcode)
        {
            if (!cpu.IsSupervisorMode())
            {
                cpu.RaiseSRException();
                return 34;
            }

            cpu.SetAddrRegisterLong(opcode & 0x07, cpu.GetUSP());
            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MoveToUsp parent;

            public AnonymousInstruction(MoveToUsp parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }

            public int Execute(int opcode)
            {
                return parent.DoMoveToUsp(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly MoveToUsp parent;

            public AnonymousInstruction1(MoveToUsp parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }

            public int Execute(int opcode)
            {
                return parent.MoveFromUsp(opcode);
            }
        }
    }
}