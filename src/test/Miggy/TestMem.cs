using System;

using M68k.CPU;
using M68k.Memory;

namespace Miggy.Memory
{
    public class TestMem : MemorySpace
    {
        public TestMem(int size) : base(size)
        {
        }

        public static TestMem Create(int size)
        {
            TestMem tm = new TestMem(size);
            return tm;
        }

        public virtual int Peek(int address, Size size)
        {
            int result;
            switch (size.Ext)
            {
                case M68k.CPU.Size.BYTESIZE:
                    result = this.ReadByte(address);
                    break;

                case M68k.CPU.Size.WORDSIZE:
                    result = this.ReadWord(address);
                    break;

                case M68k.CPU.Size.LONGSIZE:
                    result = (int)this.ReadLong(address);
                    break;

                default:
                    throw new ArgumentException("Invalid data size specified");
            }

            return (result & size.Mask);
        }

        public virtual void Poke(int address, int value, Size size)
        {
            switch (size.Ext)
            {
                case M68k.CPU.Size.BYTESIZE:
                    this.WriteByte(address, value);
                    break;

                case M68k.CPU.Size.WORDSIZE:
                    this.WriteWord(address, value);
                    break;

                case M68k.CPU.Size.LONGSIZE:
                    this.WriteLong(address, (uint)value);
                    break;
            }
        }
    }
}