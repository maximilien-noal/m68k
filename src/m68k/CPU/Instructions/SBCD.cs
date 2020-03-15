namespace M68k.CPU.Instructions
{
    public class SBCD : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SBCD(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint f = 0; f < 2; f++)
            {
                if (f == 0)
                {
                    baseAddress = 0x8100;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x8108;
                    i = new AnonymousInstruction1(this);
                }

                for (uint d = 0; d < 8; d++)
                {
                    for (uint s = 0; s < 8; s++)
                    {
                        instructionSet.AddInstruction(baseAddress + (d << 9) + s, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, bool data_reg_mode)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if (data_reg_mode)
            {
                src = new DisassembledOperand("d" + (opcode & 0x07));
                dst = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                src = new DisassembledOperand($"-(a{(opcode & 0x07)})");
                dst = new DisassembledOperand($"-(a{((opcode >> 9) & 0x07)})");
            }

            return new DisassembledInstruction(address, opcode, "sbcd", src, dst);
        }

        protected uint DoCalc(uint s, uint d)
        {
            uint x = (uint)(cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            uint c;
            uint lo = (d & 0x0f) - (s & 0x0f) - x;
            if (lo < 0)
            {
                lo += 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            uint hi = ((d >> 4) & 0x0f) - ((s >> 4) & 0x0f) - c;
            if (hi < 0)
            {
                hi += 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            uint result = (hi << 4) + lo;
            if (c != 0)
            {
                cpu.SetFlags(cpu.XFlag | cpu.CFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.XFlag | cpu.CFlag);
            }

            if (result != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            return result;
        }

        protected uint Sbcd_ar(uint opcode)
        {
            uint sreg = (opcode & 0x07);
            uint dreg = (opcode >> 9) & 0x07;
            cpu.DecrementAddrRegister(sreg, 1);
            cpu.DecrementAddrRegister(dreg, 1);
            uint s = cpu.ReadMemoryByte(cpu.GetAddrRegisterLong(sreg));
            uint d = cpu.ReadMemoryByte(cpu.GetAddrRegisterLong(dreg));
            uint result = DoCalc(s, d);
            cpu.WriteMemoryByte(cpu.GetAddrRegisterLong(dreg), result);
            return 18;
        }

        protected uint Sbcd_dr(uint opcode)
        {
            uint sreg = (opcode & 0x07);
            uint dreg = (opcode >> 9) & 0x07;
            uint s = cpu.GetDataRegisterByte(sreg);
            uint d = cpu.GetDataRegisterByte(dreg);
            uint result = DoCalc(s, d);
            cpu.SetDataRegisterByte(dreg, result);
            return 6;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SBCD parent;

            public AnonymousInstruction(SBCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }

            public uint Execute(uint opcode)
            {
                return parent.Sbcd_dr(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SBCD parent;

            public AnonymousInstruction1(SBCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }

            public uint Execute(uint opcode)
            {
                return parent.Sbcd_ar(opcode);
            }
        }
    }
}