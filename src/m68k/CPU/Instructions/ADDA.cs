namespace M68k.CPU.Instructions
{
    public class ADDA : IInstructionHandler
    {
        private readonly ICPU cpu;
        public ADDA(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xd0c0;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0xd1c0;
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(ADDA parent)
            {
                this.parent = parent;
            }

            private readonly ADDA parent;
            public uint Execute(uint opcode)
            {
                return parent.AddaWord(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(ADDA parent)
            {
                this.parent = parent;
            }

            private readonly ADDA parent;
            public uint Execute(uint opcode)
            {
                return parent.AddaLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint AddaWord(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint s = src.GetWordSigned();
            uint reg = (opcode >> 9) & 0x07;
            cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
            return 8 + src.GetTiming();
        }

        protected uint AddaLong(uint opcode)
        {
            IOperand src = cpu.ResolveSrcEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint s = src.GetLong();
            uint reg = (opcode >> 9) & 0x07;
            cpu.SetAddrRegisterLong(reg, cpu.GetAddrRegisterLong(reg) + s);
            return 6 + src.GetTiming();
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            DisassembledOperand dst = new DisassembledOperand($"a{((opcode >> 9) & 0x07)}");
            return new DisassembledInstruction(address, opcode, $"adda{sz.Ext}", src, dst);
        }
    }
}