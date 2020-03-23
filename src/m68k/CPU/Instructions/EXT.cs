namespace M68k.CPU.Instructions
{
    public class EXT : IInstructionHandler
    {
        private readonly ICPU cpu;

        public EXT(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4880;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x48c0;
                    i = new AnonymousInstruction1(this);
                }

                for (int reg = 0; reg < 8; reg++)
                {
                    instructionSet.AddInstruction(baseAddress + reg, i);
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + (opcode & 0x07));
            return new DisassembledInstruction(address, opcode, "ext" + sz.Ext, src);
        }

        protected int ExtByteToWord(int opcode)
        {
            int s = cpu.GetDataRegisterByte(opcode & 0x07);
            if ((s & 0x80) == 0x80)
            {
                s |= 0xff00;
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            cpu.SetDataRegisterWord((opcode & 0x07), s);
            if (s == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4;
        }

        protected int ExtWordToLong(int opcode)
        {
            int s = cpu.GetDataRegisterWord(opcode & 0x07);
            if ((s & 0x8000) == 0x8000)
            {
                 s |= -65536;
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            cpu.SetDataRegisterLong((opcode & 0x07), s);
            if (s == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly EXT parent;

            public AnonymousInstruction(EXT parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.ExtByteToWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly EXT parent;

            public AnonymousInstruction1(EXT parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.ExtWordToLong(opcode);
            }
        }
    }
}