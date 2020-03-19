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

            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
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

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                            break;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand op = cpu.DisassembleSrcEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"tst{sz.Ext}", op);
        }

        protected int TstByte(int opcode)
        {
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.Byte);
            int v = op.GetByte();
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

        protected int TstLong(int opcode)
        {
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.SizeLong);
            int v = op.GetLong();
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

        protected int TstWord(int opcode)
        {
            int mode = (opcode >> 3) & 0x07;
            int reg = (opcode & 0x07);
            IOperand op = cpu.ResolveSrcEA(mode, reg, Size.Word);
            int v = op.GetWord();
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

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.TstByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly TST parent;

            public AnonymousInstruction1(TST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.TstWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly TST parent;

            public AnonymousInstruction2(TST parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.TstLong(opcode);
            }
        }
    }
}