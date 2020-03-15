namespace M68k.CPU
{
    public static class CpuUtils
    {
        public static uint SignExtendByte(uint value)
        {
            uint newValue = value;
            if ((value & 0x80) == 0x80)
            {
                newValue |= 0xffffff00;
            }
            else
            {
                newValue &= 0x000000ff;
            }

            return newValue;
        }

        public static uint SignExtendWord(uint value)
        {
            uint newValue = value;
            if ((value & 0x8000) == 0x8000)
            {
                newValue |= 0xffff0000;
            }
            else
            {
                newValue &= 0x0000ffff;
            }

            return newValue;
        }
    }
}