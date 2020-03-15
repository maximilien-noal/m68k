namespace M68k.CPU
{
    public interface IInstructionHandler
    {
        void Register(IInstructionSet instructionSet);
    }
}