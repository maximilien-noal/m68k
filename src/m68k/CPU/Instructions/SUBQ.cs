namespace M68k.CPU.Instructions
{
    public class SUBQ : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SUBQ(ICPU cpu)
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
                    baseAddress = 0x5100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x5140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x5180;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (sz == 0 && ea_mode == 1)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        for (uint imm = 0; imm < 8; imm++)
                        {
                            instructionSet.AddInstruction(baseAddress + (imm << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("#" + ((opcode >> 9) & 0x07));
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "subq" + sz.Ext, src, dst);
        }

        protected uint Subq_byte(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = d - s;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected uint Subq_long(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            uint mode = (opcode >> 3) & 0x07;
            IOperand dst = cpu.ResolveDstEA(mode, (opcode & 0x07), Size.SizeLong);
            uint d = dst.GetLong();
            uint r = d - s;
            dst.SetLong(r);
            if (mode != 1)
                cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected uint Subq_word(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            uint mode = (opcode >> 3) & 0x07;
            if (mode != 1)
            {
                IOperand dst = cpu.ResolveDstEA(mode, (opcode & 0x07), Size.Word);
                uint d = dst.GetWordSigned();
                uint r = d - s;
                dst.SetWord(r);
                cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.Word);
                return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
            }
            else
            {
                uint reg = opcode & 0x07;
                cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) - s);
                return 4;
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SUBQ parent;

            public AnonymousInstruction(SUBQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subq_byte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SUBQ parent;

            public AnonymousInstruction1(SUBQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subq_word(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly SUBQ parent;

            public AnonymousInstruction2(SUBQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subq_long(opcode);
            }
        }
    }
}