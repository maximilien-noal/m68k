namespace M68k.CPU
{
    public static class CpuUtils
    {
        public static int SignExtendByte(int value)
        {
            int newValue = value;
            if ((value & 0x80) == 0x80)
            {
                unchecked
                {
                    newValue |= (int)0xffffff00;
                }
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
                unchecked
                {
                    newValue |= (int)0xffff0000;
                }
            }
            else
            {
                newValue &= 0x0000ffff;
            }

            return newValue;
        }
    }
}