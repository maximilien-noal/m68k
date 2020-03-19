namespace M68k.CPU.Instructions
{
    public class CHK : IInstructionHandler
    {
        private readonly ICPU cpu;

        public CHK(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4180;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
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

        protected int Chk(int opcode)
        {
            int reg = (opcode >> 9) & 0x07;
            int dval = cpu.GetDataRegisterWordSigned(reg);
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int sval = op.GetWord();
            bool raiseException = false;
            if (dval < 0)
            {
                cpu.SetFlags(cpu.NFlag);
                raiseException = true;
            }
            else if (dval > sval)
            {
                cpu.ClrFlags(cpu.NFlag);
                raiseException = true;
            }

            if (raiseException)
            {
                cpu.RaiseException(6);
                return 40 + op.GetTiming();
            }

            return 10 + op.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "chk", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly CHK parent;

            public AnonymousInstruction(CHK parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.Chk(opcode);
            }
        }
    }
}