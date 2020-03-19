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
            int baseAddress;
            IInstruction i;
            for (int f = 0; f < 2; f++)
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

                for (int d = 0; d < 8; d++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        instructionSet.AddInstruction(baseAddress + (d << 9) + s, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, bool dataRegMode)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            if (dataRegMode)
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

        protected int DoCalc(int s, int d)
        {
            int x = (cpu.IsFlagSet(cpu.XFlag) ? 1 : 0);
            int c;
            int lo = (d & 0x0f) - (s & 0x0f) - x;
            if (lo < 0)
            {
                lo += 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            int hi = ((d >> 4) & 0x0f) - ((s >> 4) & 0x0f) - c;
            if (hi < 0)
            {
                hi += 10;
                c = 1;
            }
            else
            {
                c = 0;
            }

            int result = (hi << 4) + lo;
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

        protected int SbcdAr(int opcode)
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

        protected int SbcdDr(int opcode)
        {
            int sreg = (opcode & 0x07);
            int dreg = (opcode >> 9) & 0x07;
            int s = cpu.GetDataRegisterByte(sreg);
            int d = cpu.GetDataRegisterByte(dreg);
            int result = DoCalc(s, d);
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, true);
            }

            public int Execute(int opcode)
            {
                return parent.SbcdDr(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SBCD parent;

            public AnonymousInstruction1(SBCD parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, false);
            }

            public int Execute(int opcode)
            {
                return parent.SbcdAr(opcode);
            }
        }
    }
}