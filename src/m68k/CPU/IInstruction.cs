namespace M68k.CPU
{
    public interface IInstruction
    {
        DisassembledInstruction Disassemble(int address, int opcode);

        int Execute(int opcode);
    }
}