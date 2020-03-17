namespace M68k.CPU.Instructions
{
    public class SUBA : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SUBA(ICPU cpu)
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
                    baseAddress = 0x90c0;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x91c0;
                    i = new AnonymousInstruction1(this);
                }

                for (uint reg = 0; reg < 8; reg++)
                {
                    for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                    {
                        for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                        {
                            if (ea_mode == 7 && ea_reg > 4)
                                break;
                            instructionSet.AddInstruction(baseAddress + (reg << 9) + (ea_mode << 3) + ea_reg, i);
                        }
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
            return new DisassembledInstruction(address, opcode, "suba" + sz.Ext, src, dst);
        }

        protected uint SubaLong(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint s = src.GetLong();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetAddrRegisterLong(reg);
            uint r = d - s;
            cpu.SetAddrRegisterLong(reg, r);
            return 6 + src.GetTiming();
        }

        protected uint SubaWord(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = src.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            uint d = cpu.GetAddrRegisterLong(reg);
            uint r = d - s;
            cpu.SetAddrRegisterLong(reg, r);
            return 8 + src.GetTiming();
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SUBA parent;

            public AnonymousInstruction(SUBA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.SubaWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SUBA parent;

            public AnonymousInstruction1(SUBA parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.SubaLong(opcode);
            }
        }
    }
}