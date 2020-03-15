using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class ANDI : IInstructionHandler
    {
        private readonly ICPU cpu;

        public ANDI(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public virtual void Register(IInstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            uint baseAddress;
            IInstruction i;
            for (uint sz = 0; sz < 3; sz++)
            {
                if (sz == 0)
                {
                    baseAddress = 0x0200;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x0240;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x0280;
                    i = new AnonymousInstruction2(this);
                }

                for (uint ea_mode = 0; ea_mode < 8; ea_mode++)
                {
                    if (ea_mode == 1)
                        continue;
                    for (uint ea_reg = 0; ea_reg < 8; ea_reg++)
                    {
                        if (ea_mode == 7 && (ea_reg == 2 || ea_reg == 3 || ea_reg == 4))
                            continue;
                        instructionSet.AddInstruction(baseAddress + (ea_mode << 3) + ea_reg, i);
                    }
                }
            }
        }

        protected virtual uint Andi_byte(uint opcode)
        {
            uint s = CpuUtils.SignExtendByte(cpu.FetchPCWord());
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Byte);
            uint d = dst.GetByteSigned();
            uint r = s & d;
            dst.SetByte(r);
            if (!dst.IsSR())
                cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Byte);
            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected virtual uint Andi_long(uint opcode)
        {
            uint s = cpu.FetchPCLong();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.SizeLong);
            uint d = dst.GetLong();
            uint r = s & d;
            dst.SetLong(r);
            cpu.CalcFlags(InstructionType.AND, s, d, r, Size.SizeLong);
            return (dst.IsRegisterMode() ? 14 : 20 + dst.GetTiming());
        }

        protected virtual uint Andi_word(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned();
            IOperand dst = cpu.ResolveDstEA((opcode >> 3) & 0x07, opcode & 0x07, Size.Word);
            uint d = dst.GetWordSigned();
            uint r = s & d;
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
                cpu.CalcFlags(InstructionType.AND, s, d, r, Size.Word);
            }

            return (dst.IsRegisterMode() ? 8 : 12 + dst.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint immBytes;
            uint imm;
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
                        throw new ArgumentException("Size unsized for ANDI");
                    }
            }

            DisassembledOperand src = new DisassembledOperand(instructionSet, immBytes, imm);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + immBytes, (opcode >> 3) & 0x07, opcode & 0x07, sz);
            return new DisassembledInstruction(address, opcode, $"andi{sz.Ext}", src, dst);
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly ANDI parent;

            public AnonymousInstruction(ANDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }

            public uint Execute(uint opcode)
            {
                return parent.Andi_byte(opcode);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            private readonly ANDI parent;

            public AnonymousInstruction1(ANDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }

            public uint Execute(uint opcode)
            {
                return parent.Andi_word(opcode);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            private readonly ANDI parent;

            public AnonymousInstruction2(ANDI parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }

            public uint Execute(uint opcode)
            {
                return parent.Andi_long(opcode);
            }
        }
    }
}