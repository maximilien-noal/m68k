namespace M68k.CPU
{
    public interface IInstructionSet
    {
        void AddInstruction(uint opcode, IInstruction i);
    }
}