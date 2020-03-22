namespace M68k.CPU.Instructions
{
    public class CMPA : IInstructionHandler
    {
        private readonly ICPU cpu;

        public CMPA(ICPU cpu)
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
                    baseAddress = 0xb0c0;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0xb1c0;
                    i = new AnonymousInstruction1(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 4)
                            break;
                        for (int r = 0; r < 8; r++)
                        {
                            instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected int CmpaLong(int opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int d = cpu.GetAddrRegisterLong((opcode >> 9) & 0x07);
            int s = op.GetLong();
            int r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return 6 + op.GetTiming();
        }

        protected int CmpaWord(int opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int d = cpu.GetAddrRegisterLong((opcode >> 9) & 0x07);
            int s = op.GetWordSigned();
            int r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return 6 + op.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "cmpa" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly CMPA parent;

            public AnonymousInstruction(CMPA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.CmpaWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly CMPA parent;

            public AnonymousInstruction1(CMPA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.CmpaLong(opcode);
            }
        }
    }
}