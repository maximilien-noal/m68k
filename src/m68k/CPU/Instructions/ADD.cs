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
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
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

                for (int reg = 0; reg < 8; reg++)
                {
                    for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        for (int ea_reg = 0; ea_reg < 8; ea_reg++)
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

            for (int sz = 0; sz < 3; sz++)
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

                for (int reg = 0; reg < 8; reg++)
                {
                    for (int ea_mode = 2; ea_mode < 8; ea_mode++)
                    {
                        for (int ea_reg = 0; ea_reg < 8; ea_reg++)
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

        protected int AddByteDnDest(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            int s = src.GetByteSigned();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetDataRegisterByteSigned(reg);
            int r = s + d;
            cpu.SetDataRegisterByte(reg, r);
            int time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return time;
        }

        protected int AddByteEaDest(int opcode)
        {
            int s = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            int d = dst.GetByteSigned();
            int r = s + d;
            dst.SetByte(r);
            int time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return time;
        }

        protected int AddLongDnDest(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int s = src.GetLong();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetDataRegisterLong(reg);
            int r = s + d;
            cpu.SetDataRegisterLong(reg, r);
            int time = 6 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return time;
        }

        protected int AddLongEaDest(int opcode)
        {
            int s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int d = dst.GetLong();
            int r = s + d;
            dst.SetLong(r);
            int time = 12 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return time;
        }

        protected int AddWordDnDest(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int s = src.GetWordSigned();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetDataRegisterWordSigned(reg);
            int r = s + d;
            cpu.SetDataRegisterWord(reg, r);
            int time = 4 + src.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
            return time;
        }

        protected int AddWordEaDest(int opcode)
        {
            int s = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int d = dst.GetWordSigned();
            int r = s + d;
            dst.SetWord(r);
            int time = 8 + dst.GetTiming();
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
            return time;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x0100) == 0)
            {
                src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
                dst = new DisassembledOperand($"d{(opcode >> 9) & 0x07}");
            }
            else
            {
                src = new DisassembledOperand($"d{(opcode >> 9) & 0x07}");
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddLongEaDest(opcode);
            }
        }
    }
}