namespace M68k.CPU.Instructions
{
    public class CLR : IInstructionHandler
    {
        private readonly ICPU cpu;

        public CLR(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x4200;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x4240;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x4280;
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

        protected int ClrByte(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            op.GetByte();
            op.SetByte(0);
            int flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected int ClrLong(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            op.GetLong();
            op.SetLong(0);
            int flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 6 : 12 + op.GetTiming());
        }

        protected int ClrWord(int opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            op.GetWord();
            op.SetWord(0);
            int flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "clr" + sz.Ext, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly CLR parent;

            public AnonymousInstruction(CLR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.ClrByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly CLR parent;

            public AnonymousInstruction1(CLR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.ClrWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly CLR parent;

            public AnonymousInstruction2(CLR parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.ClrLong(opcode);
            }
        }
    }
}