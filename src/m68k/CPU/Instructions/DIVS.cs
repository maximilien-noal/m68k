namespace M68k.CPU.Instructions
{
    public class DIVS : IInstructionHandler
    {
        private readonly ICPU cpu;

        public DIVS(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x81c0;
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "divs", src, dst);
        }

        protected uint Divs(uint opcode)
        {
            IOperand op = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            int s = (int)op.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            int d = (int)cpu.GetDataRegisterLong(reg);
            uint time;
            if (s == 0)
            {
                cpu.RaiseException(5);
                time = 38 + op.GetTiming();
            }
            else
            {
                int quot = d / s;
                if (quot > 32767 || quot < -32768)
                {
                    cpu.SetFlags(cpu.VFlag);
                }
                else
                {
                    int remain = (d % s) & 0xffff;
                    int result = (quot & 0x0000ffff) | (remain << 16);
                    cpu.SetDataRegisterLong(reg, (uint)result);
                    if ((quot & 0x8000) != 0)
                    {
                        cpu.SetFlags(cpu.NFlag);
                        cpu.ClrFlags(cpu.ZFlag);
                    }
                    else
                    {
                        cpu.ClrFlags(cpu.NFlag);
                        if (quot == 0)
                            cpu.SetFlags(cpu.ZFlag);
                        else
                            cpu.ClrFlags(cpu.ZFlag);
                    }

                    cpu.ClrFlags((cpu.VFlag | cpu.CFlag));
                }

                time = 158 + op.GetTiming();
            }

            return time;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly DIVS parent;

            public AnonymousInstruction(DIVS parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Divs(opcode);
            }
        }
    }
}