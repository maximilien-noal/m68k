namespace M68k.CPU.Instructions
{
    public class PEA : IInstructionHandler
    {
        protected static readonly uint[] TIMING = new uint[] { 0, 0, 12, 0, 0, 16, 20, 16, 20, 16, 20 };

        private readonly ICPU cpu;

        public PEA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress = 0x4840;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 3 || ea_mode == 4)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 3)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "pea", src);
        }

        protected uint Pea(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            cpu.PushLong(src.GetComputedAddress());
            return TIMING[src.Index()];
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly PEA parent;

            public AnonymousInstruction(PEA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Pea(opcode);
            }
        }
    }
}