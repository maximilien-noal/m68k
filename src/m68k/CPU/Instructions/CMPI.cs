using System;
using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class CMPI : IInstructionHandler
    {
        private readonly ICPU cpu;
        public CMPI(ICPU cpu)
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
                    baseAddress = 0x0c00;
                    i = new AnonymousInstruction(this);
                }
                else if (sz == 1)
                {
                    baseAddress = 0x0c40;
                    i = new AnonymousInstruction1(this);
                }
                else
                {
                    baseAddress = 0x0c80;
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
            public AnonymousInstruction(CMPI parent)
            {
                this.parent = parent;
            }

            private readonly CMPI parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpiByte(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Byte);
            }
        }

        private sealed class AnonymousInstruction1 : IInstruction
        {
            public AnonymousInstruction1(CMPI parent)
            {
                this.parent = parent;
            }

            private readonly CMPI parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpiWord(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.Word);
            }
        }

        private sealed class AnonymousInstruction2 : IInstruction
        {
            public AnonymousInstruction2(CMPI parent)
            {
                this.parent = parent;
            }

            private readonly CMPI parent;
            public uint Execute(uint opcode)
            {
                return parent.CmpiLong(opcode);
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                return parent.DisassembleOp(address, opcode, Size.SizeLong);
            }
        }

        protected uint CmpiByte(uint opcode)
        {
            uint s = CpuUtils.SignExtendByte(cpu.FetchPCWord());
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Byte);
            uint d = op.GetByteSigned();
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Byte);
            return (op.IsRegisterMode() ? 8 : 8 + op.GetTiming());
        }

        protected uint CmpiWord(uint opcode)
        {
            uint s = cpu.FetchPCWordSigned();
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.Word);
            uint d = op.GetWordSigned();
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.Word);
            return (op.IsRegisterMode() ? 8 : 8 + op.GetTiming());
        }

        protected uint CmpiLong(uint opcode)
        {
            uint s = cpu.FetchPCLong();
            IOperand op = cpu.ResolveDstEA((opcode >> 3) & 0x07, (opcode & 0x07), Size.SizeLong);
            uint d = op.GetLong();
            uint r = d - s;
            cpu.CalcFlags(InstructionType.CMP, s, d, r, Size.SizeLong);
            return (op.IsRegisterMode() ? 14 : 12 + op.GetTiming());
        }

        protected DisassembledInstruction DisassembleOp(uint address, uint opcode, Size sz)
        {
            uint val;
            uint bytes_read;
            string op;
            switch (sz.Ext)
            {
                case Size.BYTESIZE:
                {
                    val = cpu.ReadMemoryWord(address + 2);
                    bytes_read = 2;
                    op = $"#${(val & 0xff).ToString("x2", CultureInfo.InvariantCulture)}";
                    break;
                }

                case Size.WORDSIZE:
                {
                    val = cpu.ReadMemoryWord(address + 2);
                    bytes_read = 2;
                    op = $"#${(val & 0x0000ffff).ToString("x4", CultureInfo.InvariantCulture)}";
                    break;
                }

                case Size.LONGSIZE:
                {
                    val = cpu.ReadMemoryLong(address + 2);
                    bytes_read = 4;
                    op = $"#${val.ToString("x8", CultureInfo.InvariantCulture)}";
                    break;
                }

                default:
                {
                    throw new ArgumentException("Invalid size for CMPI");
                }
            }

            DisassembledOperand src = new DisassembledOperand(op, bytes_read, val);
            DisassembledOperand dst = cpu.DisassembleDstEA(address + 2 + bytes_read, (opcode >> 3) & 0x07, (opcode & 0x07), sz);
            return new DisassembledInstruction(address, opcode, $"cmpi{sz.Ext}", src, dst);
        }
    }
}