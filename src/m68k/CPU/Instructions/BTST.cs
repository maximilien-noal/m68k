using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class BTST : IInstructionHandler
    {
        private readonly ICPU cpu;

        public BTST(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress = 0x0100;
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
                    if (ea_mode == 7 && ea_reg > 4)
                        break;
                    for (int r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            baseAddress = 0x0800;
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
                    if (ea_mode == 7 && ea_reg > 3)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        protected int BtstDynByte(int opcode)
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

            return 4 + op.GetTiming();
        }

        protected int BtstDynLong(int opcode)
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

            return 6;
        }

        protected int BtstStaticByte(int opcode)
        {
            var bit = cpu.FetchPCWord() & 0x07;
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

            return 8 + op.GetTiming();
        }

        protected int BtstStaticLong(int opcode)
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

            return 10;
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
            return new DisassembledInstruction(address, opcode, "btst", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly BTST parent;

            public AnonymousInstruction(BTST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.BtstDynLong(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly BTST parent;

            public AnonymousInstruction1(BTST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.BtstDynByte(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly BTST parent;

            public AnonymousInstruction2(BTST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.BtstStaticLong(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly BTST parent;

            public AnonymousInstruction3(BTST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.BtstStaticByte(opcode);
            }
        }
    }
}