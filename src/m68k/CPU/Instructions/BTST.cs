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

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x0100;
            IInstruction i;
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
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

                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 4)
                        break;
                    for (uint r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            baseAddress = 0x0800;
            for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
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

                for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                {
                    if (ea_mode == 7 && ea_reg > 3)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(BTST parent)
            {
                this.parent = parent;
            }

            private readonly BTST parent;
            public uint Execute(uint opcode)
            {
                return parent.BtstDynLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(BTST parent)
            {
                this.parent = parent;
            }

            private readonly BTST parent;
            public uint Execute(uint opcode)
            {
                return parent.BtstDynByte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(BTST parent)
            {
                this.parent = parent;
            }

            private readonly BTST parent;
            public uint Execute(uint opcode)
            {
                return parent.BtstStaticLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(BTST parent)
            {
                this.parent = parent;
            }

            private readonly BTST parent;
            public uint Execute(uint opcode)
            {
                return parent.BtstStaticByte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        protected uint BtstDynByte(uint opcode)
        {
            var bit = (int)cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 7;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint val = op.GetByte();
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

        protected uint BtstDynLong(uint opcode)
        {
            var bit = (int)cpu.GetDataRegisterLong((opcode >> 9) & 0x07) & 31;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint val = op.GetLong();
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

        protected uint BtstStaticByte(uint opcode)
        {
            var bit = (int)cpu.FetchPCWord() & 0x07;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint val = op.GetByte();
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

        protected uint BtstStaticLong(uint opcode)
        {
            var bit = (int)cpu.FetchPCWord() & 31;
            bit = 1 << bit;
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint val = op.GetLong();
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand src;
            uint bytes = 2;
            if ((opcode & 0x0100) != 0)
            {
                src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
            }
            else
            {
                uint ext = cpu.ReadMemoryWord(address + 2);
                uint val;
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
    }
}