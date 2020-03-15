namespace M68k.CPU
{
    public interface IInstruction
    {
        DisassembledInstruction Disassemble(uint address, uint opcode);

        uint Execute(uint opcode);
    }
}