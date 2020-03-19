namespace M68k.CPU.Instructions
{
    public class ADDX : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ADDX(ICPU cpu)
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
                    baseAddress = 0xd100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xd140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xd180;
                    i = new AnonymousInstruction2(this);
                }

                for (int regx = 0; regx < 8; regx++)
                {
                    for (int regy = 0; regy < 8; regy++)
                    {
                        instructionSet.AddInstruction(baseAddress + (regx << 9) + regy, i);
                    }
                }
            }

            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xd108;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xd148;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0xd188;
                    i = new AnonymousInstruction5(this);
                }

                for (int regx = 0; regx < 8; regx++)
                {
                    for (int regy = 0; regy < 8; regy++)
                    {
                        instructionSet.AddInstruction(baseAddress + (regx << 9) + regy, i);
                    }
                }
            }
        }

        protected virtual int AddxByteMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 1);
            cpu.DecrementAddrRegister(ry, 1);
            int s = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(rx));
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryByte(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 18;
        }

        protected virtual int AddxByteReg(int opcode)
        {
            int s = cpu.GetDataRegisterByteSigned((opcode & 0x07));
            int d = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterByte((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 4;
        }

        protected virtual int AddxLongMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 4);
            cpu.DecrementAddrRegister(ry, 4);
            int s = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(rx));
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryLong(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 30;
        }

        protected virtual int AddxLongReg(int opcode)
        {
            int s = cpu.GetDataRegisterLong((opcode & 0x07));
            int d = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterLong((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.SizeLong);
            return 8;
        }

        protected virtual int AddxWordMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 2);
            cpu.DecrementAddrRegister(ry, 2);
            int s = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(rx));
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryWord(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Word);
            return 18;
        }

        protected virtual int AddxWordReg(int opcode)
        {
            int s = cpu.GetDataRegisterWordSigned((opcode & 0x07));
            int d = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            int r = (s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterWord((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Word);
            return 4;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x08) == 0)
            {
                src = new DisassembledOperand($"d{(opcode & 0x07)}");
                dst = new DisassembledOperand($"d{((opcode >> 9) & 0x07)}");
            }
            else
            {
                src = new DisassembledOperand($"-(a{(opcode & 0x07)})");
                dst = new DisassembledOperand($"-(a{((opcode >> 9) & 0x07)})");
            }

            return new DisassembledInstruction(address, opcode, $"addx{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AddxByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction1(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AddxWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction2(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddxLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction3(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AddxByteMem(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction4(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AddxWordMem(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly ADDX parent;

            public AnonymousInstruction5(ADDX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddxLongMem(opcode);
            }
        }
    }
}