namespace M68k.CPU.Instructions
{
    public class NBCD : IInstructionHandler
    {
        private readonly ICPU cpu;

        public NBCD(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x4800;
            IInstruction i = new AnonymousInstruction(this);
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "nbcd", op);
        }

        protected int Nbcd(int opcode)
        {
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            IOperand op = cpu.ResolveDstEA(mode, reg, Size.Byte);
            int s = op.GetByte();
            int x = (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            int c;
            int lo = 10 - (s & 0x0f) - x;
            if (lo < 10)
            {
                c = 1;
            }
            else
            {
                lo = 0;
                c = 0;
            }

            int hi = 10 - ((s >> 4) & 0x0f) - c;
            if (hi < 10)
            {
                c = 1;
            }
            else
            {
                c = 0;
                hi = 0;
            }

            int result = (hi << 4) + lo;
            if (c != 0)
            {
                cpu.SetFlags(cpu.XFlag | cpu.CFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.XFlag | cpu.CFlag);
            }

            if (result != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            op.SetByte(result);
            return (op.IsRegisterMode() ? 6 : 8);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly NBCD parent;

            public AnonymousInstruction(NBCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.Nbcd(opcode);
            }
        }
    }
}