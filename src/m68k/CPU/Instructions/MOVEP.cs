using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class MOVEP : IInstructionHandler
    {
        private readonly ICPU cpu;

        public MOVEP(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            int baseAddress;
            IInstruction i;
            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x0188;
                    i = new AnonymousInstruction(this);
                }
                else
                {
                    baseAddress = 0x01c8;
                    i = new AnonymousInstruction1(this);
                }

                for (int dreg = 0; dreg < 8; dreg++)
                {
                    for (int areg = 0; areg < 8; areg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (dreg << 9) + areg, i);
                    }
                }
            }

            for (int sz = 0; sz < 2; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x0108;
                    i = new AnonymousInstruction2(this);
                }
                else
                {
                    baseAddress = 0x0148;
                    i = new AnonymousInstruction3(this);
                }

                for (int dreg = 0; dreg < 8; dreg++)
                {
                    for (int areg = 0; areg < 8; areg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (dreg << 9) + areg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz, bool r2m)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            int dis = cpu.ReadMemoryWordSigned(address + 2);
            if (r2m)
            {
                src = new DisassembledOperand($"d{((opcode >> 9) & 0x07)}");
                dst = new DisassembledOperand($"#${dis.ToString("x4", CultureInfo.InvariantCulture)}(a{opcode & 0x07}", 2, dis);
            }
            else
            {
                src = new DisassembledOperand($"#${dis.ToString("x4", CultureInfo.InvariantCulture)}(a{opcode & 0x07})", 2, dis);
                dst = new DisassembledOperand($"d{((opcode >> 9) & 0x07)}");
            }

            return new DisassembledInstruction(address, opcode, $"movep{sz.Ext}", src, dst);
        }

        protected int M2rLong(int opcode)
        {
            int dis = cpu.FetchPCWordSigned();
            int address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            int val = cpu.ReadMemoryByte(address) << 24;
            val |= cpu.ReadMemoryByte(address + 2) << 16;
            val |= cpu.ReadMemoryByte(address + 4) << 8;
            val |= cpu.ReadMemoryByte(address + 6);
            cpu.SetDataRegisterLong((opcode >> 9) & 0x07, val);
            return 24;
        }

        protected int M2rWord(int opcode)
        {
            int dis = cpu.FetchPCWordSigned();
            int address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            int val = cpu.ReadMemoryByte(address) << 8;
            val |= cpu.ReadMemoryByte(address + 2);
            cpu.SetDataRegisterWord((opcode >> 9) & 0x07, val);
            return 16;
        }

        protected int R2mLong(int opcode)
        {
            int dis = cpu.FetchPCWordSigned();
            int address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            int val = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            cpu.WriteMemoryByte(address, (val >> 24) & 0xff);
            cpu.WriteMemoryByte(address + 2, (val >> 16) & 0xff);
            cpu.WriteMemoryByte(address + 4, (val >> 8) & 0xff);
            cpu.WriteMemoryByte(address + 6, val & 0xff);
            return 24;
        }

        protected int R2mWord(int opcode)
        {
            int dis = cpu.FetchPCWordSigned();
            int address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            int val = cpu.GetDataRegisterWord((opcode >> 9) & 0x07);
            cpu.WriteMemoryByte(address, (val >> 8) & 0xff);
            cpu.WriteMemoryByte(address + 2, val & 0xff);
            return 16;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly MOVEP parent;

            public AnonymousInstruction(MOVEP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, true);
            }

            public int Execute(int opcode)
            {
                return parent.R2mWord(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly MOVEP parent;

            public AnonymousInstruction1(MOVEP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, true);
            }

            public int Execute(int opcode)
            {
                return parent.R2mLong(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly MOVEP parent;

            public AnonymousInstruction2(MOVEP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, false);
            }

            public int Execute(int opcode)
            {
                return parent.M2rWord(opcode);
            }
        }

        private sealed class AnonymousInstruction3 : IInstruction
        {
            private readonly MOVEP parent;

            public AnonymousInstruction3(MOVEP parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, false);
            }

            public int Execute(int opcode)
            {
                return parent.M2rLong(opcode);
            }
        }
    }
}