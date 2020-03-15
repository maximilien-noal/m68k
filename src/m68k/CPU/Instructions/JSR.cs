namespace M68k.CPU.Instructions
{
    public class JSR : IInstructionHandler
    {
        private readonly ICPU cpu;
        protected static readonly uint[] TIMING = new uint[]{0, 0, 16, 0, 0, 18, 22, 18, 20, 18, 22};
        public JSR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x4e80;
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
            public AnonymousInstruction(JSR parent)
            {
                this.parent = parent;
            }

            private readonly JSR parent;
            public uint Execute(uint opcode)
            {
                return parent.Jsr(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Jsr(uint opcode)
        {
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.SizeLong);
            cpu.PushLong(cpu.GetPC());
            cpu.SetPC(op.GetComputedAddress());
            uint idx;
            if (mode == 7)
                idx = mode + reg;
            else
                idx = mode;
            return TIMING[idx];
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "jsr", op);
        }
    }
}