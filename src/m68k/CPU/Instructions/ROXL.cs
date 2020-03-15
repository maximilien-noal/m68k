namespace M68k.CPU.Instructions
{
    public class ROXL : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ROXL(ICPU cpu)
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
                    baseAddress = 0xe110;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe150;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xe190;
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
                    baseAddress = 0xe130;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xe170;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xe1b0;
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

            baseAddress = 0xe5c0;
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
                return new DisassembledInstruction(address, opcode, "roxl" + sz.Ext, src);
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

            return new DisassembledInstruction(address, opcode, "roxl" + sz.Ext, src, dst);
        }

        protected virtual uint Roxl_byte_imm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            d &= 0xff;
            cpu.SetDataRegisterByte(reg, d);
            if (xflag)
                maskFlags = cpu.XFlag + cpu.CFlag;
            else
                maskFlags = 0;
            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x80) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 6 + shift + shift;
        }

        protected virtual uint Roxl_byte_reg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterByte(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags = xflag ? cpu.XFlag : 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            d &= 0xff;
            cpu.SetDataRegisterByte(reg, d);
            if (shift != 0)
            {
                if (xflag)
                    maskFlags = cpu.XFlag + cpu.CFlag;
                else
                    maskFlags = 0;
            }
            else
            {
                if (maskFlags != 0)
                    maskFlags += cpu.CFlag;
            }

            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x80) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 6 + shift + shift;
        }

        protected virtual uint Roxl_long_imm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80000000;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            cpu.SetDataRegisterLong(reg, d);
            if (xflag)
                maskFlags = cpu.XFlag + cpu.CFlag;
            else
                maskFlags = 0;
            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x80000000) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 8 + shift + shift;
        }

        protected virtual uint Roxl_long_reg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterLong(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags = xflag ? cpu.XFlag : 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x80000000;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            cpu.SetDataRegisterLong(reg, d);
            if (shift != 0)
            {
                if (xflag)
                    maskFlags = cpu.XFlag + cpu.CFlag;
                else
                    maskFlags = 0;
            }
            else
            {
                if (maskFlags != 0)
                    maskFlags += cpu.CFlag;
            }

            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x80000000) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 8 + shift + shift;
        }

        protected virtual uint Roxl_word_imm(uint opcode)
        {
            uint shift = (opcode >> 9) & 0x07;
            if (shift == 0)
                shift = 8;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            d &= 0x0000ffff;
            cpu.SetDataRegisterWord(reg, d);
            if (xflag)
                maskFlags = cpu.XFlag + cpu.CFlag;
            else
                maskFlags = 0;
            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x8000) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 6 + shift + shift;
        }

        protected virtual uint Roxl_word_mem(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint v = op.GetWord();
            uint last_out = v & 0x8000;
            v <<= 1;
            if (cpu.IsFlagSet(cpu.XFlag))
                v |= 0x01;
            op.SetWord(v);
            uint maskFlags;
            if (last_out != 0)
                maskFlags = cpu.XFlag + cpu.CFlag;
            else
                maskFlags = 0;
            if ((v & 0xffff) == 0)
                maskFlags += cpu.ZFlag;
            if ((v & 0x8000) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 8 + op.GetTiming();
        }

        protected virtual uint Roxl_word_reg(uint opcode)
        {
            uint shift = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 63;
            uint reg = (opcode & 0x07);
            uint d = cpu.GetDataRegisterWord(reg);
            uint last_out;
            bool xflag = cpu.IsFlagSet(cpu.XFlag);
            uint maskFlags = xflag ? cpu.XFlag : 0;
            for (uint s = 0; s < shift; s++)
            {
                last_out = d & 0x8000;
                d <<= 1;
                if (xflag)
                    d |= 1;
                if (last_out != 0)
                    xflag = true;
                else
                    xflag = false;
            }

            d &= 0xffff;
            cpu.SetDataRegisterWord(reg, d);
            if (shift != 0)
            {
                if (xflag)
                    maskFlags = cpu.XFlag + cpu.CFlag;
                else
                    maskFlags = 0;
            }
            else
            {
                if (maskFlags != 0)
                    maskFlags += cpu.CFlag;
            }

            if (d == 0)
                maskFlags += cpu.ZFlag;
            if ((d & 0x8000) != 0)
                maskFlags += cpu.NFlag;
            cpu.SetCCRegister(maskFlags);
            return 6 + shift + shift;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_byte_imm(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction1(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_word_imm(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction2(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_long_imm(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction3(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_byte_reg(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction4(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_word_reg(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction5(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_long_reg(opcode);
            }
        }

        private sealed class AnonymousInstruction6 : IInstruction
        {
            private readonly ROXL parent;

            public AnonymousInstruction6(ROXL parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Roxl_word_mem(opcode);
            }
        }
    }
}