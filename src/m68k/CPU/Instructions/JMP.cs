namespace M68k.CPU.Instructions
{
    public class JMP : IInstructionHandler
    {
        private readonly ICPU cpu;
        protected static readonly uint[] TIMING = new uint[]{0, 0, 8, 0, 0, 10, 14, 10, 12, 10, 14};
        public JMP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x4ec0;
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(JMP parent)
            {
                this.parent = parent;
            }

            private readonly JMP parent;
            public uint Execute(uint opcode)
            {
                return parent.Jmp(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Jmp(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            cpu.SetPC(op.GetComputedAddress());
            return TIMING[op.Index()];
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "jmp", op);
        }
    }
}