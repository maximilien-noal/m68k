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
            uint baseAddress = 0x4800;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(NBCD parent)
            {
                this.parent = parent;
            }

            private readonly NBCD parent;
            public uint Execute(uint opcode)
            {
                return parent.Nbcd(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        protected uint Nbcd(uint opcode)
        {
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            IOperand op = cpu.ResolveDstEA(mode, reg, Size.Byte);
            uint s = op.GetByte();
            uint x = (uint)(cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            uint c;
            uint lo = 10 - (s & 0x0f) - x;
            if (lo < 10)
            {
                c = 1;
            }
            else
            {
                lo = 0;
                c = 0;
            }

            uint hi = 10 - ((s >> 4) & 0x0f) - c;
            if (hi < 10)
            {
                c = 1;
            }
            else
            {
                c = 0;
                hi = 0;
            }

            uint result = (hi << 4) + lo;
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
            return (uint)(op.IsRegisterMode() ? 6 : 8);
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "nbcd", op);
        }
    }
}