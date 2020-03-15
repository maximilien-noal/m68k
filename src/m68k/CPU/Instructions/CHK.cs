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
            uint baseAddress = 0x4180;
            IInstruction i = new AnonymousInstruction(this);
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 4)
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
            public AnonymousInstruction(CHK parent)
            {
                this.parent = parent;
            }

            private readonly CHK parent;
            public uint Execute(uint opcode)
            {
                return parent.Chk(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected uint Chk(uint opcode)
        {
            uint reg = (opcode >> 9) & 0x07;
            uint dval = cpu.GetDataRegisterWordSigned(reg);
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint sval = op.GetWord();
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "chk", src, dst);
        }
    }
}