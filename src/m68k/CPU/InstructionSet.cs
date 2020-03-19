namespace M68k.CPU
{
    public interface IInstructionSet
    {
        void AddInstruction(int opcode, IInstruction i);
    }
}