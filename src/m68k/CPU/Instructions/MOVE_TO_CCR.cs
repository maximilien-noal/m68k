namespace M68k.CPU.Instructions
{
    public class MOVE_TO_CCR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public MOVE_TO_CCR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x44c0;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 4)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(MOVE_TO_CCR parent)
            {
                this.parent = parent;
            }

            private readonly MOVE_TO_CCR parent;
            public uint Execute(uint opcode)
            {
                return parent.Move_to_ccr(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected uint Move_to_ccr(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint s = src.GetWord() & 31;
            cpu.SetCCRegister(s);
            return 12 + src.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("ccr");
            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }
    }
}