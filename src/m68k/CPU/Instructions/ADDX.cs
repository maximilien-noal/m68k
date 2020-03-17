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

            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
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

                for (uint regx = 0; regx < 8; regx++)
                {
                    for (uint regy = 0; regy < 8; regy++)
                    {
                        instructionSet.AddInstruction(baseAddress + (regx << 9) + regy, i);
                    }
                }
            }

            for (uint sz = 0; sz < 3; sz++)
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

                for (uint regx = 0; regx < 8; regx++)
                {
                    for (uint regy = 0; regy < 8; regy++)
                    {
                        instructionSet.AddInstruction(baseAddress + (regx << 9) + regy, i);
                    }
                }
            }
        }

        protected virtual uint AddxByteMem(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 1);
            cpu.DecrementAddrRegister(ry, 1);
            uint s = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ry));
            uint d = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(rx));
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryByte(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 18;
        }

        protected virtual uint AddxByteReg(uint opcode)
        {
            uint s = cpu.GetDataRegisterByteSigned((opcode & 0x07));
            uint d = cpu.GetDataRegisterByteSigned((opcode >> 9) & 0x07);
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterByte((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 4;
        }

        protected virtual uint AddxLongMem(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 4);
            cpu.DecrementAddrRegister(ry, 4);
            uint s = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ry));
            uint d = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(rx));
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryLong(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Byte);
            return 30;
        }

        protected virtual uint AddxLongReg(uint opcode)
        {
            uint s = cpu.GetDataRegisterLong((opcode & 0x07));
            uint d = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterLong((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.SizeLong);
            return 8;
        }

        protected virtual uint AddxWordMem(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            cpu.DecrementAddrRegister(rx, 2);
            cpu.DecrementAddrRegister(ry, 2);
            uint s = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ry));
            uint d = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(rx));
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.WriteMemoryWord(cpu.GetAddrRegisterLong(rx), r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Word);
            return 18;
        }

        protected virtual uint AddxWordReg(uint opcode)
        {
            uint s = cpu.GetDataRegisterWordSigned((opcode & 0x07));
            uint d = cpu.GetDataRegisterWordSigned((opcode >> 9) & 0x07);
            uint r = (uint)(s + d + (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0));
            cpu.SetDataRegisterWord((opcode >> 9) & 0x07, r);
            cpu.CalcFlags(InstructionType.ADDX, s, d, r, Size.Word);
            return 4;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.AddxLongMem(opcode);
            }
        }
    }
}