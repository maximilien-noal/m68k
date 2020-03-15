using System.Globalization;

namespace M68k.CPU.Instructions
{
    public class STOPInstruction : IInstructionHandler
    {
        private readonly ICPU cpu;

        public STOPInstruction(ICPU cpu)
        {
            this.cpu = cpu;
        }

        public void Register(IInstructionSet instructionSet)
        {
            instructionSet.AddInstruction(0x4e72, new AnonymousInstruction(this));
        }

        private sealed class AnonymousInstruction : IInstruction
        {
            private readonly STOPInstruction parent;

            public AnonymousInstruction(STOPInstruction parent)
            {
                this.parent = parent;
            }

            public DisassembledInstruction Disassemble(uint address, uint opcode)
            {
                uint data = parent.cpu.ReadMemoryWord(address + 2);
                DisassembledOperand op = new DisassembledOperand($"#${data.ToString("x4", CultureInfo.InvariantCulture)}", 2, data);
                return new DisassembledInstruction(address, opcode, "stop", op);
            }

            public uint Execute(uint opcode)
            {
                uint data = parent.cpu.FetchPCWord();
                if (!parent.cpu.IsSupervisorMode() || (data & parent.cpu.SupervisorFlag) == 0)
                {
                    parent.cpu.RaiseException(8);
                    return 34;
                }
                else
                {
                    parent.cpu.SetSR(data);
                    parent.cpu.StopNow();
                    return 4;
                }
            }
        }
    }
}