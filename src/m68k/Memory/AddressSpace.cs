using System;

namespace M68k.Memory
{
    public interface IAddressSpace : IDisposable
    {
        uint GetEndAddress();

        uint GetStartAddress();

        uint InternalReadByte(uint addr);

        uint InternalReadLong(uint addr);

        uint InternalReadWord(uint addr);

        void InternalWriteByte(uint addr, uint value);

        void InternalWriteLong(uint addr, uint value);

        void InternalWriteWord(uint addr, uint value);

        uint ReadByte(uint addr);

        uint ReadLong(uint addr);

        uint ReadWord(uint addr);

        void Reset();

        uint Size();

        void WriteByte(uint addr, uint value);

        void WriteLong(uint addr, uint value);

        void WriteWord(uint addr, uint value);
    }
}