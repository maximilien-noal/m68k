namespace M68k.CPU
{
    public interface IOperand
    {
        uint GetByte();

        uint GetByteSigned();

        uint GetComputedAddress();

        uint GetLong();

        uint GetTiming();

        uint GetWord();

        uint GetWordSigned();

        uint Index();

        void Init(uint param, Size size);

        bool IsRegisterMode();

        bool IsSR();

        void SetByte(uint value);

        void SetLong(uint value);

        void SetWord(uint value);
    }
}