namespace M68k.CPU.Instructions
{
    public class ABCD : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ABCD(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseNumber;
            IInstruction i;
            for (uint f = 0; f < 2; f++)
            {
                if (f == 0)
                {
                    baseNumber = 0xc100;
                    i = new ABCDData(this);
                }
                else
                {
                    baseNumber = 0xc108;
                    i = new ABCDReg(this);
                }

                for (uint d = 0; d < 8; d++)
                {
                    for (uint s = 0; s < 8; s++)
                    {
                        instructionSet.AddInstruction(baseNumber + (d << 9) + s, i);
                    }
                }
            }
        }

        protected uint AbcdAr(uint opcode)
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

        protected uint AbcdDr(uint opcode)
        {
            uint sreg = (opcode & 0x07);
            uint dreg = (opcode >> 9) & 0x07;
            uint s = cpu.GetDataRegisterByte(sreg);
            uint d = cpu.GetDataRegisterByte(dreg);
            uint result = DoCalc(s, d);
            cpu.SetDataRegisterByte(dreg, result);
            return 6;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, bool dataRegMode)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if (dataRegMode)
            {
                src = new DisassembledOperand($"d{(opcode & 0x07)}");
                dst = new DisassembledOperand($"d{((opcode >> 9) & 0x07)}");
            }
            else
            {
                src = new DisassembledOperand($"-(a{(opcode & 0x07)})");
                dst = new DisassembledOperand($"-(a{((opcode >> 9) & 0x07)})");
            }

            return new DisassembledInstruction(address, opcode, "abcd", src, dst);
        }

        protected uint DoCalc(uint s, uint d)
        {
            int x = (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            uint c;
            var lo = (s & 0x0f) + (d & 0x0f) + x;
            if (lo > 9)
            {
                lo -= 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            uint hi = ((s >> 4) & 0x0f) + ((d >> 4) & 0x0f) + c;
            if (hi > 9)
            {
                hi -= 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            var result = (hi << 4) + lo;
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

            return (uint)result;
        }

        private sealed class ABCDData : IInstruction
        {
            private readonly ABCD parent;

            public ABCDData(ABCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }

            public uint Execute(uint opcode)
            {
                return parent.AbcdDr(opcode);
            }
        }

        private sealed class ABCDReg : IInstruction
        {
            private readonly ABCD parent;

            public ABCDReg(ABCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }

            public uint Execute(uint opcode)
            {
                return parent.AbcdAr(opcode);
            }
        }
    }
}