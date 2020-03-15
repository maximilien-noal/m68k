namespace M68k.CPU.Instructions
{
    public class MOVE_FROM_SR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public MOVE_FROM_SR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x40c0;
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
            public AnonymousInstruction(MOVE_FROM_SR parent)
            {
                this.parent = parent;
            }

            private readonly MOVE_FROM_SR parent;
            public uint Execute(uint opcode)
            {
                return parent.Move_from_sr(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected uint Move_from_sr(uint opcode)
        {
            IOperand dst =cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            dst.GetWord();
            dst.SetWord(cpu.GetSR());
            return (dst.IsRegisterMode() ? 6 : 8 + dst.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("sr");
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "move", src, dst);
        }
    }
}