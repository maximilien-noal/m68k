namespace M68k.CPU.Instructions
{
    public class ROR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ROR(ICPU cpu)
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
                    baseAddress = 0xe018;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe058;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe098;
                    i = new AnonymousInstruction2(this);
                }

                for (uint imm = 0; imm < 8; imm++)
                {
                    for (uint reg = 0; reg < 8; reg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (imm << 9) + reg, i);
                    }
                }
            }

            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xe038;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe078;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe0b8;
                    i = new AnonymousInstruction5(this);
                }

                for (uint imm = 0; imm < 8; imm++)
                {
                    for (uint reg = 0; reg < 8; reg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (imm << 9) + reg, i);
                    }
                }
            }

            baseAddress = 0xe6c0;
            i = new AnonymousInstruction6(this);
            for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x00c0) == 0x00c0)
            {
                src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                return new DisassembledInstruction(address, opcode, "ror" + sz.Ext, src);
            }
            else if ((opcode & 0x0020) == 0x0020)
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                dst = new DisassembledOperand("d" + (opcode & 0x07));
            }
            else
            {
                uint count = (opcode >> 9) & 0x07;
                if (count == 0)
                    count = 8;
                src = new DisassembledOperand("#" + count);
                dst = new DisassembledOperand("d" + (opcode & 0x07));
            }

            return new DisassembledInstruction(address, opcode, "ror" + sz.Ext, src, dst);
        }

        protected virtual uint RorByteImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint RorByteReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint RorLongImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80000000;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint RorLongReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x80000000;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint RorWordImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x8000;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected virtual uint RorWordMem(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint v = op.GetWord();
            uint last_out = v & 0x01;
            v >>= 1;
            if (last_out != 0)
                v |= 0x8000;
            op.SetWord(v);
            cpu.CalcFlags(InstructionType.ROR, 1, last_out, v, Size.Word);
            return 8 + op.GetTiming();
        }

        protected virtual uint RorWordReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                if (last_out != 0)
                    d |= 0x8000;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ROR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorByteImm(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction1(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorWordImm(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction2(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorLongImm(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction3(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction4(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction5(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            private readonly ROR parent;

            public AnonymousInstruction6(ROR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.RorWordMem(opcode);
            }
        }
    }
}