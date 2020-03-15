namespace M68k.CPU.Instructions
{
    public class MULS : IInstructionHandler
    {
        private readonly ICPU cpu;
        public MULS(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0xc1c0;
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
            public AnonymousInstruction(MULS parent)
            {
                this.parent = parent;
            }

            private readonly MULS parent;
            public uint Execute(uint opcode)
            {
                return parent.Muls(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected uint Muls(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = op.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterWord(reg);
            if ((d & 0x8000) == 0x8000)
                d |= 0xffff0000;
            uint r = s * d;
            if (r < 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            if (r == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            cpu.ClrFlags((cpu.VFlag | cpu.CFlag));
            cpu.SetDataRegisterLong(reg, r);
            uint last_bit = 0;
            uint val;
            uint count = 0;
            for (uint x = 0; x < 16; x++)
            {
                val = s & 1;
                if (val != last_bit)
                    count++;
                last_bit = val;
                s >>= 1;
            }

            return 38 + (count << 1);
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "muls", src, dst);
        }
    }
}