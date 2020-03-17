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

            uint baseAddress;
            IInstruction i;
            for (uint t = 0; t < 2; t++)
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

                for (uint reg = 0; reg < 8; reg++)
                {
                    instructionSet.AddInstruction(baseAddress + reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(MoveToUsp parent)
            {
                this.parent = parent;
            }

            private readonly MoveToUsp parent;
            public uint Execute(uint opcode)
            {
                return parent.DoMoveToUsp(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(MoveToUsp parent)
            {
                this.parent = parent;
            }

            private readonly MoveToUsp parent;
            public uint Execute(uint opcode)
            {
                return parent.MoveFromUsp(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }
        }

        protected uint DoMoveToUsp(uint opcode)
        {
            if (!cpu.IsSupervisorMode())
            {
                cpu.RaiseSRException();
                return 34;
            }

            cpu.SetUSP(cpu.GetAddrRegisterLong(opcode & 0x07));
            return 4;
        }

        protected uint MoveFromUsp(uint opcode)
        {
            if (!cpu.IsSupervisorMode())
            {
                cpu.RaiseSRException();
                return 34;
            }

            cpu.SetAddrRegisterLong(opcode & 0x07, cpu.GetUSP());
            return 4;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, bool moveToUsp)
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
    }
}