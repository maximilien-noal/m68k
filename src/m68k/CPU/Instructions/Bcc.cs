using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class BCC : IInstructionHandler
    {
        protected static readonly string[] names = new[] { "bra", "bsr", "bhi", "bls", "bcc", "bcs", "bne", "beq", "bvc", "bvs", "bpl", "bmi", "bge", "blt", "bgt", "ble" };

        private readonly ICPU cpu;

        public BCC(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new System.ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress = 0x6000;
            IInstruction ib = new AnonymousInstruction(this);
            IInstruction iw = new AnonymousInstruction1(this);
            for (uint cc = 0; cc < 16; cc++)
            {
                instructionSet.AddInstruction(baseAddress + (cc << 8), iw);
                for (uint dis = 1; dis < 256; dis++)
                {
                    instructionSet.AddInstruction(baseAddress + (cc << 8) + dis, ib);
                }
            }
        }

        protected uint BxxByte(uint opcode)
        {
            uint dis = CpuUtils.SignExtendByte(opcode & 0xff);
            uint cc = (opcode >> 8) & 0x0f;
            uint pc = cpu.GetPC();
            uint time;
            if (cc == 1)
            {
                cpu.PushLong(pc);
                cpu.SetPC(pc + dis);
                time = 18;
            }
            else
            {
                if (cpu.TestCC(cc))
                {
                    cpu.SetPC(pc + dis);
                    time = 10;
                }
                else
                {
                    time = 8;
                }
            }

            return time;
        }

        protected uint BxxWord(uint opcode)
        {
            uint pc = cpu.GetPC();
            uint dis = cpu.ReadMemoryWordSigned(pc);
            uint cc = (opcode >> 8) & 0x0f;
            uint time;
            if (cc == 1)
            {
                cpu.PushLong(pc + 2);
                cpu.SetPC(pc + dis);
                time = 18;
            }
            else
            {
                if (cpu.TestCC(cc))
                {
                    cpu.SetPC(pc + dis);
                    time = 10;
                }
                else
                {
                    time = 12;
                    cpu.SetPC(pc + 2);
                }
            }

            return time;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode)
        {
            DisassembledOperand op;
            uint cc = (opcode >> 8) & 0x0f;
            uint dis = CpuUtils.SignExtendByte(opcode & 0xff);
            string name;
            if (dis != 0)
            {
                op = new DisassembledOperand($"{(dis + address + 2).ToString("x8", CultureInfo.InvariantCulture)}");
                name = $"{names[cc]}.s";
            }
            else
            {
                dis = cpu.ReadMemoryWordSigned(address + 2);
                op = new DisassembledOperand($"{(dis + address + 2).ToString("x8", CultureInfo.InvariantCulture)}", 2, dis);
                name = $"{names[cc]}.w";
            }

            return new DisassembledInstruction(address, opcode, name, op);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly BCC parent;

            public AnonymousInstruction(BCC parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.BxxByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly BCC parent;

            public AnonymousInstruction1(BCC parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode);
            }

            public uint Execute(uint opcode)
            {
                return parent.BxxWord(opcode);
            }
        }
    }
}