using System;

namespace M68k.CPU.Instructions
{
    public class TST : IInstructionHandler
    {
        private readonly ICPU cpu;

        public TST(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4a00;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x4a40;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x4a80;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "tst" + sz.Ext, op);
        }

        protected uint Tst_byte(uint opcode)
        {
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.Byte);
            uint v = op.GetByte();
            if (v == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            if ((v & 0x080) != 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4 + op.GetTiming();
        }

        protected uint Tst_long(uint opcode)
        {
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.SizeLong);
            uint v = op.GetLong();
            if (v == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            if ((v & 0x80000000) != 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4 + op.GetTiming();
        }

        protected uint Tst_word(uint opcode)
        {
            uint mode = (opcode >> 3) & 0x07;
            uint reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.Word);
            uint v = op.GetWord();
            if (v == 0)
            {
                cpu.SetFlags(cpu.ZFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.ZFlag);
            }

            if ((v & 0x8000) != 0)
            {
                cpu.SetFlags(cpu.NFlag);
            }
            else
            {
                cpu.ClrFlags(cpu.NFlag);
            }

            cpu.ClrFlags(cpu.CFlag | cpu.VFlag);
            return 4 + op.GetTiming();
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly TST parent;

            public AnonymousInstruction(TST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Tst_byte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly TST parent;

            public AnonymousInstruction1(TST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Tst_word(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly TST parent;

            public AnonymousInstruction2(TST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Tst_long(opcode);
            }
        }
    }
}