namespace M68k.CPU.Instructions
{
    public class SUBX : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SUBX(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x9100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x9140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x9180;
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
                    baseAddress = 0x9108;
                    i = new AnonymousInstruction3(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x9148;
                    i = new AnonymousInstruction4(this);
                }
                else
                {
                    baseAddress = 0x9188;
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

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if ((opcode & 0x08) == 0)
            {
                src = new DisassembledOperand("d" + (opcode & 0x07));
                dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                src = new DisassembledOperand($"-(a{opcode & 0x07})");
                dst = new DisassembledOperand($"-(a{(opcode >> 9) & 0x07})");
            }

            return new DisassembledInstruction(address, opcode, "subx" + sz.Ext, src, dst);
        }

        protected virtual int SubxByteMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 1);
            cpu.DecrementAddrRegister(ry, 1);
            int s = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(rx));
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryByte(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.Byte);
            return 18;
        }

        protected virtual int SubxByteReg(int opcode)
        {
            int s = cpu.GetDataRegisterByteSigned((opcode & 0x07));
            int d = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterByte((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.Byte);
            return 4;
        }

        protected virtual int SubxLongMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 4);
            cpu.DecrementAddrRegister(ry, 4);
            int s = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(rx));
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryLong(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.Byte);
            return 30;
        }

        protected virtual int SubxLongReg(int opcode)
        {
            int s = cpu.GetDataRegisterLong((opcode & 0x07));
            int d = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterLong((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.SizeLong);
            return 8;
        }

        protected virtual int SubxWordMem(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 2);
            cpu.DecrementAddrRegister(ry, 2);
            int s = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ry));
            int d = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(rx));
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryWord(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.Word);
            return 18;
        }

        protected virtual int SubxWordReg(int opcode)
        {
            int s = cpu.GetDataRegisterWordSigned((opcode & 0x07));
            int d = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            int r = (d - s - (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterWord((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.SUBX, s, d, r, Size.Word);
            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.SubxByteReg(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction1(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.SubxWordReg(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction2(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.SubxLongReg(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction3(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.SubxByteMem(opcode);
            }
        }

        private sealed class AnonymousInstruction4 : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction4(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.SubxWordMem(opcode);
            }
        }

        private sealed class AnonymousInstruction5 : IInstruction
        {
            private readonly SUBX parent;

            public AnonymousInstruction5(SUBX parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.SubxLongMem(opcode);
            }
        }
    }
}