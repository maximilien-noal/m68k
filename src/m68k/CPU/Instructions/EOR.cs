namespace M68k.CPU.Instructions
{
    public class EOR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public EOR(ICPU cpu)
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
                    baseAddress = 0xb100;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xb140;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xb180;
                    i = new AnonymousInstruction2(this);
                }

                for (int reg = 0; reg < 8; reg++)
                {
                    for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        if (ea_mode == 1)
                            continue;
                        for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 1)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "eor" + sz.Ext, src, dst);
        }

        protected int EorByte(int opcode)
        {
            int s = cpu.GetDataRegisterByte((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            int d = dst.GetByte();
            int r = s ^ d;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        protected int EorLong(int opcode)
        {
            int s = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int d = dst.GetLong();
            int r = s ^ d;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected int EorWord(int opcode)
        {
            int s = cpu.GetDataRegisterWord((opcode >> 9) & 0x07);
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int d = dst.GetWord();
            int r = s ^ d;
            dst.SetWord(r);
            cpu.CalcFlags(InstructionType.EOR, s, d, r, Size.Word);
            return (dst.IsRegisterMode() ? 4 : 8 + dst.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly EOR parent;

            public AnonymousInstruction(EOR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.EorByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly EOR parent;

            public AnonymousInstruction1(EOR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.EorWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly EOR parent;

            public AnonymousInstruction2(EOR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.EorLong(opcode);
            }
        }
    }
}