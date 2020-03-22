namespace Miggy
{
    using M68k.CPU;

    using Miggy.Memory;

    using System;

    public class TestCpu : MC68000
    {
        public TestCpu(TestMem mem) : base()
        {
            AddressSpace = mem;
        }

        public virtual int Fetch(Size size)
        {
            switch (size.Ext)
            {
                case Size.BYTESIZE:
                case Size.WORDSIZE:
                    return FetchPCWord();

                case Size.LONGSIZE:
                    return FetchPCLong();
            }

            return base.FetchPCLong();
        }

        public virtual int GetAddrRegister(int reg)
        {
            return base.GetAddrRegisterLong(reg);
        }

        public virtual int GetDataRegister(int reg)
        {
            return base.GetDataRegisterLong(reg);
        }

        public virtual bool IsSet(int flag)
        {
            return base.IsFlagSet(flag);
        }

        public virtual void Push(int value, Size size)
        {
            switch (size.Ext)
            {
                case Size.BYTESIZE:
                    throw new InvalidOperationException("Not implemented");
                case Size.WORDSIZE:
                    PushWord(value);
                    break;

                case Size.LONGSIZE:
                    PushLong(value);
                    break;
            }
        }

        public virtual void SetAddrRegister(int reg, int value, Size size)
        {
            switch (size.Ext)
            {
                case Size.BYTESIZE:
                    SetAddrRegisterByte(reg, value);
                    break;

                case Size.WORDSIZE:
                    SetAddrRegisterWord(reg, value);
                    break;

                case Size.LONGSIZE:
                    SetAddrRegisterLong(reg, value);
                    break;
            }
        }

        public virtual void SetAddrRegister(int reg, int value)
        {
            Size size = GetSize(value);
            SetAddrRegister(reg, value, size);
        }

        public virtual void SetCCR(int value)
        {
            base.SetCCRegister(value);
        }

        public virtual void SetDataRegister(int reg, int value, Size size)
        {
            switch (size.Ext)
            {
                case Size.BYTESIZE:
                    SetDataRegisterByte(reg, value);
                    break;

                case Size.WORDSIZE:
                    SetDataRegisterWord(reg, value);
                    break;

                case Size.LONGSIZE:
                    SetDataRegisterLong(reg, value);
                    break;
            }
        }

        public virtual void SetDataRegister(int reg, int value)
        {
            Size size = GetSize(value);
            SetDataRegister(reg, value, size);
        }

        private static Size GetSize(int val)
        {
            if ((val & 0xFFFF) != val)
            {
                return Size.SizeLong;
            }

            if ((val & 0xFF) != val)
            {
                return Size.Word;
            }

            return Size.Byte;
        }
    }
}