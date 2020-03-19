using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class BSET : IInstructionHandler
    {
        private readonly ICPU cpu;

        public BSET(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x01c0;
            IInstruction i;
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                if (ea_mode == 0)
                {
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    i = new AnonymousInstruction1(this);
                }

                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    for (int r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            baseAddress = 0x08c0;
            for (int ea_mode = 0; ea_mode < 8; ea_mode++)
            {
                if (ea_mode == 1)
                    continue;
                if (ea_mode == 0)
                {
                    i = new AnonymousInstruction2(this);
                }
                else
                {
                    i = new AnonymousInstruction3(this);
                }

                for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected int BsetDynByte(int opcode)
        {
            var bit = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 7;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            int val = op.GetByte();
            if ((val & bit) != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }
            else
            {
                cpu.SetFlags(cpu.ZFlag);
            }

            val |= (bit);
            op.SetByte(val);
            return 8 + op.GetTiming();
        }

        protected int BsetDynLong(int opcode)
        {
            var bit = cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 31;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int val = op.GetLong();
            if ((val & bit) != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }
            else
            {
                cpu.SetFlags(cpu.ZFlag);
            }

            val |= (bit);
            op.SetLong(val);
            return 10;
        }

        protected int BsetStaticByte(int opcode)
        {
            var bit = cpu.FetchPCWord() & 7;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            int val = op.GetByte();
            if ((val & bit) != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }
            else
            {
                cpu.SetFlags(cpu.ZFlag);
            }

            val |= (bit);
            op.SetByte(val);
            return 12 + op.GetTiming();
        }

        protected int BsetStaticLong(int opcode)
        {
            var bit = cpu.FetchPCWord() & 31;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            int val = op.GetLong();
            if ((val & bit) != 0)
            {
                cpu.ClrFlags(cpu.ZFlag);
            }
            else
            {
                cpu.SetFlags(cpu.ZFlag);
            }

            val |= (bit);
            op.SetLong(val);
            return 14;
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand src;
            int bytes = 2;
            if ((opcode & 0x0100) != 0)
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                int ext = cpu.ReadMemoryWord(address + 2);
                int val;
                if (((opcode >> 3) & 0x07) == 0)
                {
                    val = ext & 0x1f;
                }
                else
                {
                    val = ext & 0x07;
                }

                src = new DisassembledOperand($"#${val.ToString("x", CultureInfo.InvariantCulture)}", 2, ext);
                bytes += 2;
            }

            DisassembledOperand dst = cpu.DisassembleDstEA(address + bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "bset", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly BSET parent;

            public AnonymousInstruction(BSET parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.BsetDynLong(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly BSET parent;

            public AnonymousInstruction1(BSET parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.BsetDynByte(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly BSET parent;

            public AnonymousInstruction2(BSET parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.BsetStaticLong(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly BSET parent;

            public AnonymousInstruction3(BSET parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.BsetStaticByte(opcode);
            }
        }
    }
}