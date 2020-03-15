using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class SUBI : IInstructionHandler
    {
        private readonly ICPU cpu;

        public SUBI(ICPU cpu)
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
                    baseAddress = 0x0400;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x0440;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x0480;
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

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint imm_bytes;
            uint imm;
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
                        throw new ArgumentException("Size unsized for SUBI");
                    }
            }

            DisassembledOperand src = new DisassembledOperand(instructionSet, imm_bytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + imm_bytes, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, "subi" + sz.Ext, src, dst);
        }

        protected virtual uint Subi_byte(uint opcode)
        {
            uint s = CpuUtils.SignExtendByte(cpu.FetchPCWord());
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = d - s;
            dst.SetByte(r);
            cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected virtual uint Subi_long(uint opcode)
        {
            uint s = cpu.FetchPCLong();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = d - s;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 16 : 20 + dst.GetTiming());
        }

        protected virtual uint Subi_word(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWordSigned();
            uint r = d - s;
            dst.SetWord(r);
            cpu.CalcFlags(InstructionType.SUB, s, d, r, Size.Word);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly SUBI parent;

            public AnonymousInstruction(SUBI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subi_byte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly SUBI parent;

            public AnonymousInstruction1(SUBI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subi_word(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly SUBI parent;

            public AnonymousInstruction2(SUBI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Subi_long(opcode);
            }
        }
    }
}