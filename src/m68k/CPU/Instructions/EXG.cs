using System;

namespace M68k.CPU.Instructions
{
    public class EXG : IInstructionHandler
    {
        private readonly ICPU cpu;
        protected enum ExgMode
        {
            ExgData,
            ExgAddr,
            ExgDataAddr
        }

        public EXG(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            uint baseAddress;
            IInstruction i;
            for (uint mode = 0; mode < 3; mode++)
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

                for (uint rx = 0; rx < 8; rx++)
                {
                    for (uint ry = 0; ry < 8; ry++)
                    {
                        instructionSet.AddInstruction(baseAddress + (rx << 9) + ry, i);
                    }
                }
            }
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            public AnonymousInstruction(EXG parent)
            {
                this.parent = parent;
            }

            private readonly EXG parent;
            public uint Execute(uint opcode)
            {
                return parent.ExgDd(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgData);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(EXG parent)
            {
                this.parent = parent;
            }

            private readonly EXG parent;
            public uint Execute(uint opcode)
            {
                return parent.ExgAa(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgAddr);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(EXG parent)
            {
                this.parent = parent;
            }

            private readonly EXG parent;
            public uint Execute(uint opcode)
            {
                return parent.ExgDa(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, ExgMode.ExgDataAddr);
            }
        }

        protected uint ExgDd(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            uint x = cpu.GetDataRegisterLong(rx);
            uint y = cpu.GetDataRegisterLong(ry);
            cpu.SetDataRegisterLong(rx, y);
            cpu.SetDataRegisterLong(ry, x);
            return 6;
        }

        protected uint ExgAa(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            uint x = cpu.GetAddrRegisterLong(rx);
            uint y = cpu.GetAddrRegisterLong(ry);
            cpu.SetAddrRegisterLong(rx, y);
            cpu.SetAddrRegisterLong(ry, x);
            return 6;
        }

        protected uint ExgDa(uint opcode)
        {
            uint rx = (opcode >> 9) & 0x07;
            uint ry = (opcode & 0x07);
            uint x = cpu.GetDataRegisterLong(rx);
            uint y = cpu.GetAddrRegisterLong(ry);
            cpu.SetDataRegisterLong(rx, y);
            cpu.SetAddrRegisterLong(ry, x);
            return 6;
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, ExgMode mode)
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
    }
}