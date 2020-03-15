namespace M68k.CPU.Instructions
{
    public class ADDQ : IInstructionHandler
    {
        private readonly ICPU cpu;
        public ADDQ(ICPU cpu)
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
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x5000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x5040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x5080;
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(ADDQ parent)
            {
                this.parent = parent;
            }

            private readonly ADDQ parent;
            public uint Execute(uint opcode)
            {
                return parent.Addq_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(ADDQ parent)
            {
                this.parent = parent;
            }

            private readonly ADDQ parent;
            public uint Execute(uint opcode)
            {
                return parent.Addq_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(ADDQ parent)
            {
                this.parent = parent;
            }

            private readonly ADDQ parent;
            public uint Execute(uint opcode)
            {
                return parent.Addq_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Addq_byte(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = s + d;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected uint Addq_word(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            uint mode = (opcode >> 3) & 0x07;
            if (mode != 1)
            {
                IOperand dst =cpu.ResolveDstEA(mode, (opcode & 0x07), Size.Word);
                uint d = dst.GetWordSigned();
                uint r = s + d;
                dst.SetWord(r);
                cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
                return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
            }
            else
            {
                uint reg = opcode & 0x07;
                cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
                return 4;
            }
        }

        protected uint Addq_long(uint opcode)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            uint mode = (opcode >> 3) & 0x07;
            IOperand dst =cpu.ResolveDstEA(mode, (opcode & 0x07), Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s + d;
            dst.SetLong(r);
            if (mode != 1)
                cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            DisassembledOperand src = new DisassembledOperand("#" + s);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "addq" + sz.Ext, src, dst);
        }
    }
}