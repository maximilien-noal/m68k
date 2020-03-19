namespace M68k.CPU.Instructions
{
    public class ADDQ : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ADDQ(ICPU cpu)
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
                    baseAddress = 0x5000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x5040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x5080;
                    i = new AnonymousInstruction2(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (sz == 0 && ea_mode == 1)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        for (int imm = 0; imm < 8; imm++)
                        {
                            instructionSet.AddInstruction(baseAddress + (imm << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected int AddqByte(int opcode)
        {
            int s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            int d = dst.GetByteSigned();
            int r = s + d;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected int AddqLong(int opcode)
        {
            int s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            int mode = (opcode >> 3) & 0x07;
            IOperand dst = cpu.ResolveDstEA(mode, (opcode & 0x07), Size.SizeLong);
            int d = dst.GetLong();
            int r = s + d;
            dst.SetLong(r);
            if (mode != 1)
                cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected int AddqWord(int opcode)
        {
            int s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            int mode = (opcode >> 3) & 0x07;
            if (mode != 1)
            {
                IOperand dst = cpu.ResolveDstEA(mode, (opcode & 0x07), Size.Word);
                int d = dst.GetWordSigned();
                int r = s + d;
                dst.SetWord(r);
                cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
                return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
            }
            else
            {
                int reg = opcode & 0x07;
                cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
                return 4;
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int s = (opcode >> 9 & 0x07);
            if (s == 0)
                s = 8;
            DisassembledOperand src = new DisassembledOperand("#" + s);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "addq" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ADDQ parent;

            public AnonymousInstruction(ADDQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AddqByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ADDQ parent;

            public AnonymousInstruction1(ADDQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AddqWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ADDQ parent;

            public AnonymousInstruction2(ADDQ parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddqLong(opcode);
            }
        }
    }
}