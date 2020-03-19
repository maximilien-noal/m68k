namespace M68k.CPU.Instructions
{
    public class MOVE : IInstructionHandler
    {
        internal static readonly int[][] LongExecutionTime = new int[][] { new int[] { 4, 4, 12, 12, 12, 16, 18, 16, 20 }, new int[] { 4, 4, 12, 12, 12, 16, 18, 16, 20 }, new int[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 }, new int[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 }, new int[] { 14, 14, 22, 22, 22, 26, 28, 26, 30 }, new int[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new int[] { 18, 18, 26, 26, 26, 30, 32, 30, 34 }, new int[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new int[] { 20, 20, 28, 28, 28, 32, 34, 32, 36 }, new int[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new int[] { 18, 18, 26, 26, 26, 30, 32, 30, 34 }, new int[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 } };

        internal static readonly int[][] ShortExecutionTime = new int[][] { new int[] { 4, 4, 8, 8, 8, 12, 14, 12, 16 }, new int[] { 4, 4, 8, 8, 8, 12, 14, 12, 16 }, new int[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 }, new int[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 }, new int[] { 10, 10, 14, 14, 14, 18, 20, 18, 22 }, new int[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new int[] { 14, 14, 18, 18, 18, 22, 24, 22, 26 }, new int[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new int[] { 16, 16, 20, 20, 20, 24, 26, 24, 28 }, new int[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new int[] { 14, 14, 18, 18, 18, 22, 24, 22, 26 }, new int[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 } };

        private readonly ICPU cpu;

        public MOVE(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x1000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x3000;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x2000;
                    i = new AnonymousInstruction2(this);
                }

                for (int sea_mode = 0; sea_mode < 8; sea_mode++)
                {
                    for (int sea_reg = 0; sea_reg < 8; sea_reg++)
                    {
                        if (sea_mode == 7 && sea_reg > 4)
                            break;
                        for (int dea_mode = 0; dea_mode < 8; dea_mode++)
                        {
                            if (dea_mode == 1)
                                continue;
                            for (int dea_reg = 0; dea_reg < 8; dea_reg++)
                            {
                                if (dea_mode == 7 && dea_reg > 1)
                                    break;
                                instructionSet.AddInstruction(baseAddress + (dea_reg << 9) + (dea_mode << 6) + (sea_mode << 3) + sea_reg, i);
                            }
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + src.Bytes, (opcode >> 6) & 0x07, (opcode >> 9) & 0x07, sz);
            return new DisassembledInstruction(address, opcode, "move" + sz.Ext, src, dst);
        }

        protected int MoveByte(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.Byte);
            int s = src.GetByte();
            dst.SetByte(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.Byte);
            return ShortExecutionTime[src.Index()][dst.Index()];
        }

        protected int MoveLong(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.SizeLong);
            int s = src.GetLong();
            dst.SetLong(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.SizeLong);
            return LongExecutionTime[src.Index()][dst.Index()];
        }

        protected int MoveWord(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.Word);
            int s = src.GetWord();
            dst.SetWord(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.Word);
            return ShortExecutionTime[src.Index()][dst.Index()];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MOVE parent;

            public AnonymousInstruction(MOVE parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.MoveByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly MOVE parent;

            public AnonymousInstruction1(MOVE parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.MoveWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly MOVE parent;

            public AnonymousInstruction2(MOVE parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.MoveLong(opcode);
            }
        }
    }
}