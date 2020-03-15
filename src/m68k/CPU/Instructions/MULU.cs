namespace M68k.CPU.Instructions
{
    public class MULU : IInstructionHandler
    {
        private readonly ICPU cpu;
        public MULU(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0xc0c0;
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
            public AnonymousInstruction(MULU parent)
            {
                this.parent = parent;
            }

            private readonly MULU parent;
            public uint Execute(uint opcode)
            {
                return parent.Mulu(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        protected uint Mulu(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = op.GetWord();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetDataRegisterWord(reg);
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
            uint x = s;
            x = (x & 0x5555) + ((x >> 1) & 0x5555);
            x = (x & 0x3333) + ((x >> 2) & 0x3333);
            x = (x & 0x0f0f) + ((x >> 4) & 0x0f0f);
            x = (x & 0x00ff) + ((x >> 8) & 0x00ff);
            return 38 + (x << 1);
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "mulu", src, dst);
        }
    }
}