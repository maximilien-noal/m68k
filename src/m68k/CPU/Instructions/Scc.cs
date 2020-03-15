namespace M68k.CPU.Instructions
{
    public class Scc : IInstructionHandler
    {
        protected static readonly string[] names = new[] { "st", "sf", "shi", "sls", "scc", "scs", "sne", "seq", "svc", "svs", "spl", "smi", "sge", "slt", "sgt", "sle" };

        private readonly ICPU cpu;

        public Scc(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x50c0;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    for (uint cc = 0; cc < 16; cc++)
                    {
                        instructionSet.AddInstruction(baseAddress + (cc << 8) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            DisassembledOperand op = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            return new DisassembledInstruction(address, opcode, names[(opcode >> 8) & 0x0f], op);
        }

        protected uint Sxx(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint cc = (opcode >> 8) & 0x0f;
            uint time;
            if (cpu.TestCC(cc))
            {
                op.SetByte(0xff);
                time = (op.IsRegisterMode() ? 6 : 8 + op.GetTiming());
            }
            else
            {
                op.SetByte(0);
                time = (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
            }

            return time;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly Scc parent;

            public AnonymousInstruction(Scc parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.Sxx(opcode);
            }
        }
    }
}