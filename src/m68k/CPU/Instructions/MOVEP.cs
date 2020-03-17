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

            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 2; sz++)
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

                for (uint dreg = 0; dreg < 8; dreg++)
                {
                    for (uint areg = 0; areg < 8; areg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (dreg << 9) + areg, i);
                    }
                }
            }

            for (uint sz = 0; sz < 2; sz++)
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

                for (uint dreg = 0; dreg < 8; dreg++)
                {
                    for (uint areg = 0; areg < 8; areg++)
                    {
                        instructionSet.AddInstruction(baseAddress + (dreg << 9) + areg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz, bool r2m)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            uint dis = cpu.ReadMemoryWordSigned(address + 2);
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

        protected uint M2rLong(uint opcode)
        {
            uint dis = cpu.FetchPCWordSigned();
            uint address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            uint val = cpu.ReadMemoryByte(address) << 24;
            val |= cpu.ReadMemoryByte(address + 2) << 16;
            val |= cpu.ReadMemoryByte(address + 4) << 8;
            val |= cpu.ReadMemoryByte(address + 6);
            cpu.SetDataRegisterLong((opcode >> 9) & 0x07, val);
            return 24;
        }

        protected uint M2rWord(uint opcode)
        {
            uint dis = cpu.FetchPCWordSigned();
            uint address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            uint val = cpu.ReadMemoryByte(address) << 8;
            val |= cpu.ReadMemoryByte(address + 2);
            cpu.SetDataRegisterWord((opcode >> 9) & 0x07, val);
            return 16;
        }

        protected uint R2mLong(uint opcode)
        {
            uint dis = cpu.FetchPCWordSigned();
            uint address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            uint val = cpu.GetDataRegisterLong((opcode >> 9) & 0x07);
            cpu.WriteMemoryByte(address, (val >> 24) & 0xff);
            cpu.WriteMemoryByte(address + 2, (val >> 16) & 0xff);
            cpu.WriteMemoryByte(address + 4, (val >> 8) & 0xff);
            cpu.WriteMemoryByte(address + 6, val & 0xff);
            return 24;
        }

        protected uint R2mWord(uint opcode)
        {
            uint dis = cpu.FetchPCWordSigned();
            uint address = cpu.GetAddrRegisterLong(opcode & 0x07) + dis;
            uint val = cpu.GetDataRegisterWord((opcode >> 9) & 0x07);
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, true);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, true);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word, false);
            }

            public uint Execute(uint opcode)
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

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong, false);
            }

            public uint Execute(uint opcode)
            {
                return parent.M2rLong(opcode);
            }
        }
    }
}