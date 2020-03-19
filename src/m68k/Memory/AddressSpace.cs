namespace M68k.Memory
{
    public interface IAddressSpace
    {
        int GetEndAddress();

        int GetStartAddress();

        byte InternalReadByte(int addr);

        uint InternalReadLong(int addr);

        int InternalReadWord(int addr);

        void InternalWriteByte(int addr, int value);

        void InternalWriteLong(int addr, uint value);

        void InternalWriteWord(int addr, int value);

        byte ReadByte(int addr);

        uint ReadLong(int addr);

        int ReadWord(int addr);

        void Reset();

        int Size();

        void WriteByte(int addr, int value);

        void WriteLong(int addr, uint value);

        void WriteWord(int addr, int value);
    }
}