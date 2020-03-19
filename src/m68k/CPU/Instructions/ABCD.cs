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

            int baseNumber;
            IInstruction i;
            for (int f = 0; f < 2; f++)
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

                for (int d = 0; d < 8; d++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        instructionSet.AddInstruction(baseNumber + (d << 9) + s, i);
                    }
                }
            }
        }

        protected int AbcdAr(int opcode)
        {
            int sreg = (opcode & 0x07);
            int dreg = (opcode >> 9) & 0x07;
            cpu.DecrementAddrRegister(sreg, 1);
            cpu.DecrementAddrRegister(dreg, 1);
            int s = cpu.ReadMemoryByte(cpu.GetAddrRegisterLong(sreg));
            int d = cpu.ReadMemoryByte(cpu.GetAddrRegisterLong(dreg));
            int result = DoCalc(s, d);
            cpu.WriteMemoryByte(cpu.GetAddrRegisterLong(dreg), result);
            return 18;
        }

        protected int AbcdDr(int opcode)
        {
            int sreg = (opcode & 0x07);
            int dreg = (opcode >> 9) & 0x07;
            int s = cpu.GetDataRegisterByte(sreg);
            int d = cpu.GetDataRegisterByte(dreg);
            int result = DoCalc(s, d);
            cpu.SetDataRegisterByte(dreg, result);
            return 6;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, bool dataRegMode)
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

        protected int DoCalc(int s, int d)
        {
            int x = (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            int c;
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

            int hi = ((s >> 4) & 0x0f) + ((d >> 4) & 0x0f) + c;
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

            return result;
        }

        private sealed class ABCDData : IInstruction
        {
            private readonly ABCD parent;

            public ABCDData(ABCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }

            public int Execute(int opcode)
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }

            public int Execute(int opcode)
            {
                return parent.AbcdAr(opcode);
            }
        }
    }
}