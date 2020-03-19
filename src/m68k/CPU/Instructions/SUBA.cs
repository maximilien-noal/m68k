namespace M68k.CPU.Instructions
{
    public class SUBA : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SUBA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x90c0;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x91c0;
                    i = new AnonymousInstruction1(this);
                }

                for (int reg = 0; reg < 8; reg++)
                {
                    for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 4)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "suba" + sz.Ext, src, dst);
        }

        protected int SubaLong(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int s = src.GetLong();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetAddrRegisterLong(reg);
            int r = d - s;
            cpu.SetAddrRegisterLong(reg, r);
            return 6 + src.GetTiming();
        }

        protected int SubaWord(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = src.GetWordSigned();
            int reg = (opcode >> 9) & 0x07;
            int d = cpu.GetAddrRegisterLong(reg);
            int r = d - s;
            cpu.SetAddrRegisterLong(reg, r);
            return 8 + src.GetTiming();
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SUBA parent;

            public AnonymousInstruction(SUBA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.SubaWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SUBA parent;

            public AnonymousInstruction1(SUBA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.SubaLong(opcode);
            }
        }
    }
}