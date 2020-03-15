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
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 2; sz++)
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

                for (uint reg = 0; reg < 8; reg++)
                {
                    instructionSet.AddInstruction(baseAddress + reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(EXT parent)
            {
                this.parent = parent;
            }

            private readonly EXT parent;
            public uint Execute(uint opcode)
            {
                return parent.Ext_byte_to_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(EXT parent)
            {
                this.parent = parent;
            }

            private readonly EXT parent;
            public uint Execute(uint opcode)
            {
                return parent.Ext_word_to_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Ext_byte_to_word(uint opcode)
        {
            uint s = cpu.GetDataRegisterByte(opcode & 0x07);
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

        protected uint Ext_word_to_long(uint opcode)
        {
            uint s = cpu.GetDataRegisterWord(opcode & 0x07);
            if ((s & 0x8000) == 0x8000)
            {
                s |= 0xffff0000;
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand("d" + (opcode & 0x07));
            return new DisassembledInstruction(address, opcode, "ext" + sz.Ext, src);
        }
    }
}