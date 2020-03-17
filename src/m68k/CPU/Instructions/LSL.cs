namespace M68k.CPU.Instructions
{
    public class LSL : IInstructionHandler
    {
        private readonly ICPU cpu;
        public LSL(ICPU cpu)
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
                    baseAddress = 0xe108;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe148;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe188;
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
                    baseAddress = 0xe128;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe168;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe1a8;
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

            baseAddress = 0xe3c0;
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslByteImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslWordImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslLongImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslByteReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            public AnonymousInstruction4(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslWordReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            public AnonymousInstruction5(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslLongReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            public AnonymousInstruction6(LSL parent)
            {
                this.parent = parent;
            }

            private readonly LSL parent;
            public uint Execute(uint opcode)
            {
                return parent.LslWordMem(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected virtual uint LslByteImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint LslWordImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected virtual uint LslLongImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80000000;
                d <<= 1;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint LslByteReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint LslWordReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected virtual uint LslLongReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80000000;
                d <<= 1;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.LSL, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint LslWordMem(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint v = op.GetWord();
            uint last_out = v & 0x8000;
            v <<= 1;
            op.SetWord(v);
            cpu.CalcFlags(InstructionType.LSL, 1, last_out, v, Size.Word);
            return 8 + op.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x00c0) == 0x00c0)
            {
                src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                return new DisassembledInstruction(address, opcode, "lsl" + sz.Ext, src);
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

            return new DisassembledInstruction(address, opcode, "lsl" + sz.Ext, src, dst);
        }
    }
}