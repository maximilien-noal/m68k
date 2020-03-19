namespace M68k.CPU.Instructions
{
    public class MOVEA : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MOVEA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x3040;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x2040;
                    i = new AnonymousInstruction1(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 4)
                            break;
                        for (int r = 0; r < 8; r++)
                        {
                            instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "movea" + sz.Ext, src, dst);
        }

        protected int MoveaLong(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetLong());
            return MOVE.LongExecutionTime[src.Index()][1];
        }

        protected int MoveaWord(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetWordSigned());
            return MOVE.ShortExecutionTime[src.Index()][1];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MOVEA parent;

            public AnonymousInstruction(MOVEA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.MoveaWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly MOVEA parent;

            public AnonymousInstruction1(MOVEA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.MoveaLong(opcode);
            }
        }
    }
}