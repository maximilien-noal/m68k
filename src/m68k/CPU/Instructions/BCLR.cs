using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class BCLR : IInstructionHandler
    {
        private readonly ICPU cpu;
        public BCLR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            uint baseAddress = 0x0180;
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
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    for (uint r = 0; r < 8; r++)
                    {
                        instructionSet.AddInstruction(baseAddress + (r << 9) + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }

            baseAddress = 0x0880;
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
                    if (ea_mode == 7 && ea_reg > 1)
                        break;
                    instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(BCLR parent)
            {
                this.parent = parent;
            }

            private readonly BCLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Bclr_dyn_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(BCLR parent)
            {
                this.parent = parent;
            }

            private readonly BCLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Bclr_dyn_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(BCLR parent)
            {
                this.parent = parent;
            }

            private readonly BCLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Bclr_static_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            public AnonymousInstruction3(BCLR parent)
            {
                this.parent = parent;
            }

            private readonly BCLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Bclr_static_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        protected uint Bclr_dyn_byte(uint opcode)
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

            val &= (uint)(~bit);
            op.SetByte(val);
            return 8 + op.GetTiming();
        }

        protected uint Bclr_dyn_long(uint opcode)
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

            val &= (uint)(~bit);
            op.SetLong(val);
            return 10;
        }

        protected uint Bclr_static_byte(uint opcode)
        {
            var bit = (int)cpu.FetchPCWord() & 7;
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

            val &= (uint)(~bit);
            op.SetByte(val);
            return 12 + op.GetTiming();
        }

        protected uint Bclr_static_long(uint opcode)
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

            val &= (uint)(~bit);
            op.SetLong(val);
            return 14;
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
            return new DisassembledInstruction(address, opcode, "bclr", src, dst);
        }
    }
}