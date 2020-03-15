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
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 2; sz++)
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

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 4)
                            break;
                        for (uint r = 0; r < 8; r++)
                        {
                            instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(MOVEA parent)
            {
                this.parent = parent;
            }

            private readonly MOVEA parent;
            public uint Execute(uint opcode)
            {
                return parent.Movea_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(MOVEA parent)
            {
                this.parent = parent;
            }

            private readonly MOVEA parent;
            public uint Execute(uint opcode)
            {
                return parent.Movea_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Movea_word(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetWordSigned());
            return MOVE.ShortExecutionTime[src.Index()][1];
        }

        protected uint Movea_long(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetLong());
            return MOVE.LongExecutionTime[src.Index()][1];
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "movea" + sz.Ext, src, dst);
        }
    }
}