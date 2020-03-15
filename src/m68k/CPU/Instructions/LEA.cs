namespace M68k.CPU.Instructions
{
    public class LEA : IInstructionHandler
    {
        private readonly ICPU cpu;
        protected static readonly uint[] TIMING = new uint[]{0, 0, 4, 0, 0, 8, 12, 8, 12, 8, 12};
        public LEA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x41c0;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 2; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 3 || ea_mode == 4)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 3)
                        break;
                    for (uint r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(LEA parent)
            {
                this.parent = parent;
            }

            private readonly LEA parent;
            public uint Execute(uint opcode)
            {
                return parent.Lea(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Lea(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            cpu.SetAddrRegisterLong((opcode >> 9) & 0x07, src.GetComputedAddress());
            return TIMING[src.Index()];
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "lea", src, dst);
        }
    }
}