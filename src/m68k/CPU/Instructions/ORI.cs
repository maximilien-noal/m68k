using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class ORI : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ORI(ICPU cpu)
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
                    baseAddress = 0x0000;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x0040;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x0080;
                    i = new AnonymousInstruction2(this);
                }

                for (int ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (int ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && (ea_reg == 2 || ea_reg == 3 || ea_reg == 4))
                            continue;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected DisassembledInstruction DisassembleOp(int address, int opcode, Size sz)
        {
            int imm_bytes;
            int imm;
            string instructionSet;
            switch (sz.Ext)
            {
                case Size.BYTESIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${(imm & 0x00ff).ToString("x2", CultureInfo.InvariantCulture)}";
                        imm_bytes = 2;
                        break;
                    }

                case Size.WORDSIZE:
                    {
                        imm = cpu.ReadMemoryWord(address + 2);
                        instructionSet = $"#${imm.ToString("x4", CultureInfo.InvariantCulture)}";
                        imm_bytes = 2;
                        break;
                    }

                case Size.LONGSIZE:
                    {
                        imm = cpu.ReadMemoryLong(address + 2);
                        instructionSet = $"#${imm.ToString("x8", CultureInfo.InvariantCulture)}";
                        imm_bytes = 4;
                        break;
                    }

                default:
                    {
                        throw new ArgumentException("Size unsized for ORI");
                    }
            }

            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "ori" + sz.Ext, src, dst);
        }

        protected virtual int OriByte(int opcode)
        {
            int s = CpuUtils.SignExtendByte(cpu.FetchPCWord());
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            int d = dst.GetByteSigned();
            int r = s | d;
            dst.SetByte(r);
            if (!dst.IsSR())
                cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected virtual int OriLong(int opcode)
        {
            int s = cpu.FetchPCLong();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            int d = dst.GetLong();
            int r = s | d;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.OR, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 16 : 20 + dst.GetTiming());
        }

        protected virtual int OriWord(int opcode)
        {
            int s = cpu.FetchPCWordSigned();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            int d = dst.GetWordSigned();
            int r = s | d;
            if (dst.IsSR())
            {
                if (cpu.IsSupervisorMode())
                {
                    cpu.SetSR(r);
                }
                else
                {
                    cpu.RaiseSRException();
                    return 34;
                }
            }
            else
            {
                dst.SetWord(r);
                cpu.CalcFlags(InstructionType.OR, s, d, r, Size.Word);
            }

            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ORI parent;

            public AnonymousInstruction(ORI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public int Execute(int opcode)
            {
                return parent.OriByte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ORI parent;

            public AnonymousInstruction1(ORI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public int Execute(int opcode)
            {
                return parent.OriWord(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ORI parent;

            public AnonymousInstruction2(ORI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(int address, int opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public int Execute(int opcode)
            {
                return parent.OriLong(opcode);
            }
        }
    }
}