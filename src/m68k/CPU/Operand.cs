namespace M68k.CPU
{
    public interface IOperand
    {
        int GetByte();

        int GetByteSigned();

        int GetComputedAddress();

        int GetLong();

        int GetTiming();

        int GetWord();

        int GetWordSigned();

        int Index();

        void Init(int param, Size size);

        bool IsRegisterMode();

        bool IsSR();

        void SetByte(int value);

        void SetLong(int value);

        void SetWord(int value);
    }
}