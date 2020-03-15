namespace M68k.CPU.Instructions
{
    public class TAS : IInstructionHandler
    {
        private readonly ICPU cpu;

        private readonly object lockObject = new object();

        public TAS(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public static bool EmulateBrokenTAS { get; internal set; }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x4ac0;
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "tas", op);
        }

        protected uint Tas(uint opcode)
        {
            lock (lockObject)
            {
                uint mode = (opcode >> 3) & 0x07;
                uint reg = (opcode & 0x07);
                IOperand op = cpu.ResolveSrcEA(mode, reg, Size.Byte);
                uint v = op.GetByte();
                if (v == 0)
                {
                    cpu.SetFlags(cpu.ZFlag);
                }
                else
                {
                    cpu.ClrFlags(cpu.ZFlag);
                }

                if ((v & 0x080) != 0)
                {
                    cpu.SetFlags(cpu.NFlag);
                }
                else
                {
                    cpu.ClrFlags(cpu.NFlag);
                }

                cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
                if (!EmulateBrokenTAS)
                {
                    op.SetByte(v | 0x80);
                }

                return (op.IsRegisterMode() ? 4 : 14 + op.GetTiming());
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly TAS parent;

            public AnonymousInstruction(TAS parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Tas(opcode);
            }
        }
    }
}