using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class ADDI : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ADDI(ICPU cpu)
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
                    baseAddress = 0x0600;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x0640;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x0680;
                    i = new AnonymousInstruction2(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                    {
                        continue;
                    }

                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && ea_reg > 1)
                        {
                            break;
                        }

                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected int AddiByte(int opcode)
        {
            int s = CpuUtils.SignExtendByte(cpu.FetchPCWord());
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            int d = dst.GetByteSigned();
            int r = s + d;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected int AddiLong(int opcode)
        {
            int s = cpu.FetchPCLong();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int d = dst.GetLong();
            int r = s + d;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 16 : 20 + dst.GetTiming());
        }

        protected int AddiWord(int opcode)
        {
            int s = cpu.FetchPCWordSigned();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int d = dst.GetWordSigned();
            int r = s + d;
            dst.SetWord(r);
            cpu.CalcFlags(InstructionType.ADD, s, d, r, Size.Word);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int immBytes;
            int imm;
            string instructionSet;
            switch (sz.Ext)
            {
                case Size.BYTESIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${(imm & 0x00ff).ToString("x2", CultureInfo.InvariantCulture)}";
                        immBytes = 2;
                        break;
                    }

                case Size.WORDSIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${imm.ToString("x4", CultureInfo.InvariantCulture)}";
                        immBytes = 2;
                        break;
                    }

                case Size.LONGSIZE:
                    {
                        imm = cpu.ReadMemoryLong(address + 2);
                        instructionSet = $"#${imm.ToString("x8", CultureInfo.InvariantCulture)}";
                        immBytes = 4;
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Size unsized for ADDI");
                    }
            }

            DisassembledOperand src = new DisassembledOperand(instructionSet, immBytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + immBytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"addi{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ADDI parent;

            public AnonymousInstruction(ADDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.AddiByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ADDI parent;

            public AnonymousInstruction1(ADDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.AddiWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ADDI parent;

            public AnonymousInstruction2(ADDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.AddiLong(opcode);
            }
        }
    }
}