namespace M68k.CPU
{
    public static class CpuUtils
    {
        public static int SignExtendByte(int value)
        {
            int newValue = value;
            if ((value & 0x80) == 0x80)
            {
                newValue |= -256;
            }
            else
            {
                newValue &= 0x000000ff;
            }

            return newValue;
        }

        public static int SignExtendWord(int value)
        {
            int newValue = value;
            if ((value & 0x8000) == 0x8000)
            {
                newValue |= -65536;
            }
            else
            {
                newValue &= 0x0000ffff;
            }

            return newValue;
        }
    }
}