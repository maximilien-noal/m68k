namespace M68k.CPU.Instructions
{
    public class MOVE : IInstructionHandler
    {
        private readonly ICPU cpu;
        internal static readonly uint[][] ShortExecutionTime = new uint[][] { new uint[] { 4, 4, 8, 8, 8, 12, 14, 12, 16 }, new uint[] { 4, 4, 8, 8, 8, 12, 14, 12, 16 }, new uint[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 }, new uint[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 }, new uint[] { 10, 10, 14, 14, 14, 18, 20, 18, 22 }, new uint[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new uint[] { 14, 14, 18, 18, 18, 22, 24, 22, 26 }, new uint[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new uint[] { 16, 16, 20, 20, 20, 24, 26, 24, 28 }, new uint[] { 12, 12, 16, 16, 16, 20, 22, 20, 24 }, new uint[] { 14, 14, 18, 18, 18, 22, 24, 22, 26 }, new uint[] { 8, 8, 12, 12, 12, 16, 18, 16, 20 } };
        internal static readonly uint[][] LongExecutionTime = new uint[][] { new uint[] { 4, 4, 12, 12, 12, 16, 18, 16, 20 }, new uint[] { 4, 4, 12, 12, 12, 16, 18, 16, 20 }, new uint[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 }, new uint[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 }, new uint[] { 14, 14, 22, 22, 22, 26, 28, 26, 30 }, new uint[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new uint[] { 18, 18, 26, 26, 26, 30, 32, 30, 34 }, new uint[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new uint[] { 20, 20, 28, 28, 28, 32, 34, 32, 36 }, new uint[] { 16, 16, 24, 24, 24, 28, 30, 28, 32 }, new uint[] { 18, 18, 26, 26, 26, 30, 32, 30, 34 }, new uint[] { 12, 12, 20, 20, 20, 24, 26, 24, 28 } };
        public MOVE(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
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

                for (uint sea_mode = 0; sea_mode < 8; sea_mode++)
                {
                    for (uint sea_reg = 0; sea_reg < 8; sea_reg++)
                    {
                        if (sea_mode == 7 && sea_reg > 4)
                            break;
                        for (uint dea_mode = 0; dea_mode < 8; dea_mode++)
                        {
                            if (dea_mode == 1)
                                continue;
                            for (uint dea_reg = 0; dea_reg < 8; dea_reg++)
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(MOVE parent)
            {
                this.parent = parent;
            }

            private readonly MOVE parent;
            public uint Execute(uint opcode)
            {
                return parent.Move_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(MOVE parent)
            {
                this.parent = parent;
            }

            private readonly MOVE parent;
            public uint Execute(uint opcode)
            {
                return parent.Move_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(MOVE parent)
            {
                this.parent = parent;
            }

            private readonly MOVE parent;
            public uint Execute(uint opcode)
            {
                return parent.Move_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Move_byte(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.Byte);
            uint s = src.GetByte();
            dst.SetByte(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.Byte);
            return ShortExecutionTime[src.Index()][dst.Index()];
        }

        protected uint Move_word(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.Word);
            uint s = src.GetWord();
            dst.SetWord(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.Word);
            return ShortExecutionTime[src.Index()][dst.Index()];
        }

        protected uint Move_long(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            IOperand dst = cpu.ResolveDstEA((opcode >> 6) & 0x07, (opcode >> 9) & 0x07, Size.SizeLong);
            uint s = src.GetLong();
            dst.SetLong(s);
            cpu.CalcFlags(InstructionType.MOVE, s, s, s, Size.SizeLong);
            return LongExecutionTime[src.Index()][dst.Index()];
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + src.Bytes, (opcode >> 6) & 0x07, (opcode >> 9) & 0x07, sz);
            return new DisassembledInstruction(address, opcode, "move" + sz.Ext, src, dst);
        }
    }
}