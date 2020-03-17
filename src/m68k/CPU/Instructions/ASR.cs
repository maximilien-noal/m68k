namespace M68k.CPU.Instructions
{
    public class ASR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public ASR(ICPU cpu)
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
                    baseAddress = 0xe000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe080;
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
                    baseAddress = 0xe020;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe060;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe0a0;
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

            baseAddress = 0xe0c0;
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
            public AnonymousInstruction(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrByteImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrWordImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrLongImm(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrByteReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            public AnonymousInstruction4(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrWordReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            public AnonymousInstruction5(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrLongReg(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            public AnonymousInstruction6(ASR parent)
            {
                this.parent = parent;
            }

            private readonly ASR parent;
            public uint Execute(uint opcode)
            {
                return parent.AsrWordMem(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected virtual uint AsrByteImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint msb = d & 0x80;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint AsrWordImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint msb = d & 0x8000;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected virtual uint AsrLongImm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint msb = d & 0x80000000;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint AsrByteReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint msb = d & 0x80;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            d &= 0x00ff;
            cpu.SetDataRegisterByte(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.Byte);
            return 6 + shift + shift;
        }

        protected virtual uint AsrWordReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint msb = d & 0x8000;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.Word);
            return 6 + shift + shift;
        }

        protected virtual uint AsrLongReg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint msb = d & 0x80000000;
            uint last_out = 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x01;
                d >>= 1;
                d |= msb;
            }

            cpu.SetDataRegisterLong(reg, d);
            cpu.CalcFlags(InstructionType.ASR, shift, last_out, d, Size.SizeLong);
            return 8 + shift + shift;
        }

        protected virtual uint AsrWordMem(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint v = op.GetWord();
            uint last_out = v & 0x01;
            uint msb = v & 0x8000;
            v >>= 1;
            v |= msb;
            op.SetWord(v);
            cpu.CalcFlags(InstructionType.ASR, 1, last_out, v, Size.Word);
            return 8 + op.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x00c0) == 0x00c0)
            {
                src = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                return new DisassembledInstruction(address, opcode, "asr" + sz.Ext, src);
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

            return new DisassembledInstruction(address, opcode, "asr" + sz.Ext, src, dst);
        }
    }
}