namespace M68k.CPU.Instructions
{
    public class EOR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public EOR(ICPU cpu)
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
                    baseAddress = 0xb100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xb140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xb180;
                    i = new AnonymousInstruction2(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        if (ea_mode == 1)
                            continue;
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 1)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(EOR parent)
            {
                this.parent = parent;
            }

            private readonly EOR parent;
            public uint Execute(uint opcode)
            {
                return parent.Eor_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(EOR parent)
            {
                this.parent = parent;
            }

            private readonly EOR parent;
            public uint Execute(uint opcode)
            {
                return parent.Eor_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(EOR parent)
            {
                this.parent = parent;
            }

            private readonly EOR parent;
            public uint Execute(uint opcode)
            {
                return parent.Eor_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Eor_byte(uint opcode)
        {
            uint s = cpu.GetDataRegisterByte((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByte();
            uint r = s ^ d;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected uint Eor_word(uint opcode)
        {
            uint s = cpu.GetDataRegisterWord((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWord();
            uint r = s ^ d;
            dst.SetWord(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.Word);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected uint Eor_long(uint opcode)
        {
            uint s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s ^ d;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "eor" + sz.Ext, src, dst);
        }
    }
}