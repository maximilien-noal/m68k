namespace M68k.CPU.Instructions
{
    public class ADDA : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ADDA(ICPU cpu)
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
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xd0c0;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0xd1c0;
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

        protected int AddaLong(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int s = src.GetLong();
            int reg = (opcode >> 9) & 0x07;
            cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
            return 6 + src.GetTiming();
        }

        protected int AddaWord(int opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = src.GetWordSigned();
            int reg = (opcode >> 9) & 0x07;
            cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
            return 8 + src.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand($"a{((opcode >> 9) & 0x07)}");
            return new DisassembledInstruction(address, opcode, $"adda{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ADDA parent;

            public AnonymousInstruction(ADDA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AddaWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ADDA parent;

            public AnonymousInstruction1(ADDA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddaLong(opcode);
            }
        }
    }
}