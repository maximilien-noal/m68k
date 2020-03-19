using System;

namespace M68k.CPU.Instructions
{
    public class EXG : IInstructionHandler
    {
        private readonly ICPU cpu;

        public EXG(ICPU cpu)
        {
            this.cpu = cpu;
        }

        protected enum ExgMode
        {
            ExgData,

            ExgAddr,

            ExgDataAddr
        }

        public void Register(IInstructionSet instructionSet)
        {
            int baseAddress;
            IInstruction i;
            for (int mode = 0; mode < 3; mode++)
            {
                if (mode == 0)
                {
                    baseAddress = 0xc140;
                    i = new AnonymousInstruction(this);
                }
                else if (mode == 1)
                {
                    baseAddress = 0xc148;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0xc188;
                    i = new AnonymousInstruction2(this);
                }

                for (int rx = 0; rx < 8; rx++)
                {
                    for (int ry = 0; ry < 8; ry++)
                    {
                        instructionSet.AddInstruction(baseAddress + (rx << 9) + ry, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, ExgMode mode)
        {
            DisassembledOperand src;
            DisassembledOperand dst;
            switch (mode)
            {
                case ExgMode.ExgData:
                    {
                        src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                        dst = new DisassembledOperand("d" + (opcode & 0x07));
                        break;
                    }

                case ExgMode.ExgAddr:
                    {
                        src = new DisassembledOperand("a" + ((opcode >> 9) & 0x07));
                        dst = new DisassembledOperand("a" + (opcode & 0x07));
                        break;
                    }

                case ExgMode.ExgDataAddr:
                    {
                        src = new DisassembledOperand("d" + ((opcode >> 9) & 0x07));
                        dst = new DisassembledOperand("a" + (opcode & 0x07));
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Invalid exg type specified");
                    }
            }

            return new DisassembledInstruction(address, opcode, "exg", src, dst);
        }

        protected int ExgAa(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            int x = cpu.GetAddrRegisterLong(rx);
            int y = cpu.GetAddrRegisterLong(ry);
            cpu.SetAddrRegisterLong(rx, y);
            cpu.SetAddrRegisterLong(ry, x);
            return 6;
        }

        protected int ExgDa(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            int x = cpu.GetDataRegisterLong(rx);
            int y = cpu.GetAddrRegisterLong(ry);
            cpu.SetDataRegisterLong(rx, y);
            cpu.SetAddrRegisterLong(ry, x);
            return 6;
        }

        protected int ExgDd(int opcode)
        {
            int rx = (opcode >> 9) & 0x07;
            int ry = (opcode & 0x07);
            int x = cpu.GetDataRegisterLong(rx);
            int y = cpu.GetDataRegisterLong(ry);
            cpu.SetDataRegisterLong(rx, y);
            cpu.SetDataRegisterLong(ry, x);
            return 6;
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly EXG parent;

            public AnonymousInstruction(EXG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgData);
            }

            public int Execute(int opcode)
            {
                return parent.ExgDd(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly EXG parent;

            public AnonymousInstruction1(EXG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgAddr);
            }

            public int Execute(int opcode)
            {
                return parent.ExgAa(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly EXG parent;

            public AnonymousInstruction2(EXG parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgDataAddr);
            }

            public int Execute(int opcode)
            {
                return parent.ExgDa(opcode);
            }
        }
    }
}