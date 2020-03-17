namespace M68k.CPU.Instructions
{
    public class CMP : IInstructionHandler
    {
        private readonly ICPU cpu;
        public CMP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xb000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xb040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xb080;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1 && sz == 0)
                        continue;
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
            public AnonymousInstruction(CMP parent)
            {
                this.parent = parent;
            }

            private readonly CMP parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpByte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(CMP parent)
            {
                this.parent = parent;
            }

            private readonly CMP parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpWord(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(CMP parent)
            {
                this.parent = parent;
            }

            private readonly CMP parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint CmpByte(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint s = op.GetByte();
            uint d = cpu.GetDataRegisterByte((opcode >> 9) & 0x07);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Byte);
            return 4 + op.GetTiming();
        }

        protected uint CmpWord(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = op.GetWord();
            uint d = cpu.GetDataRegisterWord((opcode >> 9) & 0x07);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Word);
            return 4 + op.GetTiming();
        }

        protected uint CmpLong(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint s = op.GetLong();
            uint d = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return 6 + op.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "cmp" + sz.Ext, src, dst);
        }
    }
}