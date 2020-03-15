namespace M68k.CPU.Instructions
{
    public class ADD : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ADD(ICPU cpu)
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
                    baseAddress = 0xd000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xd040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xd080;
                    i = new AnonymousInstruction2(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 4)
                            {
                                break;
                            }

                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }

            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xd100;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xd140;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xd180;
                    i = new AnonymousInstruction5(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
                    {
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 1)
                            {
                                break;
                            }

                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected uint AddByteDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint s = src.GetByteSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterByteSigned(reg);
            uint r = s + d;
            cpu.SetDataRegisterByte(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return time;
        }

        protected uint AddByteEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = s + d;
            dst.SetByte(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return time;
        }

        protected uint AddLongDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint s = src.GetLong();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterLong(reg);
            uint r = s + d;
            cpu.SetDataRegisterLong(reg, r);
            uint time = 6 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return time;
        }

        protected uint AddLongEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s + d;
            dst.SetLong(r);
            uint time = 12 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return time;
        }

        protected uint AddWordDnDest(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint s = src.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterWordSigned(reg);
            uint r = s + d;
            cpu.SetDataRegisterWord(reg, r);
            uint time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
            return time;
        }

        protected uint AddWordEaDest(uint opcode)
        {
            uint s = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWordSigned();
            uint r = s + d;
            dst.SetWord(r);
            uint time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
            return time;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x0100) == 0)
            {
                src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            }

            return new DisassembledInstruction(address, opcode, "add" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddByteDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction1(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddWordDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction2(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddLongDnDest(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction3(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddByteEaDest(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction4(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddWordEaDest(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ADD parent;

            public AnonymousInstruction5(ADD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddLongEaDest(opcode);
            }
        }
    }
}