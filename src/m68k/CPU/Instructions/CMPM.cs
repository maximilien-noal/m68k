namespace M68k.CPU.Instructions
{
    public class CMPM : IInstructionHandler
    {
        private readonly ICPU cpu;

        public CMPM(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0xb108;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0xb148;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xb188;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ax = 0; ax < 8; ax++)
                {
                    for (uint ay = 0; ay < 8; ay++)
                    {
                        instructionSet.AddInstruction(baseAddress + (ax << 9) + ay, i);
                    }
                }
            }
        }

        protected uint CmpmByte(uint opcode)
        {
            uint ax = (opcode >> 9) & 0x07;
            uint ay = (opcode & 0x07);
            uint s = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 1);
            uint d = cpu.ReadMemoryByteSigned(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 1);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Byte);
            return 12;
        }

        protected uint CmpmLong(uint opcode)
        {
            uint ax = (opcode >> 9) & 0x07;
            uint ay = (opcode & 0x07);
            uint s = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 4);
            uint d = cpu.ReadMemoryLong(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 4);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return 20;
        }

        protected uint CmpmWord(uint opcode)
        {
            uint ax = (opcode >> 9) & 0x07;
            uint ay = (opcode & 0x07);
            uint s = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ay));
            cpu.IncrementAddrRegister(ay, 2);
            uint d = cpu.ReadMemoryWordSigned(cpu.GetAddrRegisterLong(ax));
            cpu.IncrementAddrRegister(ax, 2);
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Word);
            return 12;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src = new DisassembledOperand($"(a{(opcode & 0x07)})+");
            DisassembledOperand dst = new DisassembledOperand($"(a{((opcode >> 9) & 0x07)})+");
            return new DisassembledInstruction(address, opcode, "cmpm" + sz.Ext, src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.CmpmByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction1(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.CmpmWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly CMPM parent;

            public AnonymousInstruction2(CMPM parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.CmpmLong(opcode);
            }
        }
    }
}