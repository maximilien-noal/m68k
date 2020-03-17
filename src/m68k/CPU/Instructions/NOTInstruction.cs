namespace M68k.CPU.Instructions
{
    public class NOTInstruction : IInstructionHandler
    {
        private readonly ICPU cpu;
        public NOTInstruction(ICPU cpu)
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
                    baseAddress = 0x4600;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x4640;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x4680;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(NOTInstruction parent)
            {
                this.parent = parent;
            }

            private readonly NOTInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.NotByte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(NOTInstruction parent)
            {
                this.parent = parent;
            }

            private readonly NOTInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.NotWord(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(NOTInstruction parent)
            {
                this.parent = parent;
            }

            private readonly NOTInstruction parent;
            public uint Execute(uint opcode)
            {
                return parent.NotLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint NotByte(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint s = op.GetByte();
            uint r = (~s) & 0xFF;
            op.SetByte(r);
            cpu.CalcFlags(InstructionType.NOT, s, 0, r, Size.Byte);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected uint NotWord(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = op.GetWord();
            uint r = (~s) & 0xffff;
            op.SetWord(r);
            cpu.CalcFlags(InstructionType.NOT, s, 0, r, Size.Word);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected uint NotLong(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint s = op.GetLong();
            uint r = ~s;
            op.SetLong(r);
            cpu.CalcFlags(InstructionType.NOT, s, 0, r, Size.SizeLong);
            return (op.IsRegisterMode() ? 6 : 12 + op.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "not" + sz.Ext, src);
        }
    }
}