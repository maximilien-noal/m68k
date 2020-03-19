namespace M68k.CPU.Instructions
{
    public class CMPM : IInstructionHandler
    {
        private readonly ICPU cpu;

        public CMPM(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xb108;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xb148;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xb188;
                    i = new AnonymousInstruction2(this);
                }

                for (int ax = 0; ax < 8; ax++)
                {
                    for (int ay = 0; ay < 8; ay++)
                    {
                        instructionSet.AddInstruction(baseAddress + (ax << 9) + ay, i);
                    }
                }
            }
        }

        protected int CmpmByte(int opcode)
        {
            int ax = (opcode >> 9) & 0x07;
            int ay = (opcode & 0x07);
            int s = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 1);
            int d = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 1);
            int r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Byte);
            return 12;
        }

        protected int CmpmLong(int opcode)
        {
            int ax = (opcode >> 9) & 0x07;
            int ay = (opcode & 0x07);
            int s = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 4);
            int d = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 4);
            int r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return 20;
        }

        protected int CmpmWord(int opcode)
        {
            int ax = (opcode >> 9) & 0x07;
            int ay = (opcode & 0x07);
            int s = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 2);
            int d = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 2);
            int r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Word);
            return 12;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand($"(a{(opcode & 0x07)})+");
            DisassembledOperand dst = new DisassembledOperand($"(a{((opcode >> 9) & 0x07)})+");
            return new DisassembledInstruction(address, opcode, "cmpm" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.CmpmByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction1(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.CmpmWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction2(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.CmpmLong(opcode);
            }
        }
    }
}