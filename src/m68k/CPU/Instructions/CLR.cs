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
            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
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

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(CLR parent)
            {
                this.parent = parent;
            }

            private readonly CLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Clr_byte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(CLR parent)
            {
                this.parent = parent;
            }

            private readonly CLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Clr_word(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(CLR parent)
            {
                this.parent = parent;
            }

            private readonly CLR parent;
            public uint Execute(uint opcode)
            {
                return parent.Clr_long(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint Clr_byte(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            op.GetByte();
            op.SetByte(0);
            uint flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected uint Clr_word(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            op.GetWord();
            op.SetWord(0);
            uint flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 4 : 8 + op.GetTiming());
        }

        protected uint Clr_long(uint opcode)
        {
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            op.GetLong();
            op.SetLong(0);
            uint flags = cpu.GetCCRegister();
            flags &= ~(cpu.NFlag | cpu.CFlag | cpu.VFlag);
            flags |= cpu.ZFlag;
            cpu.SetCCRegister(flags);
            return (op.IsRegisterMode() ? 6 : 12 + op.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "clr" + sz.Ext, dst);
        }
    }
}