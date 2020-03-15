using System;

namespace M68k.CPU
{
    public class CpuException : Exception
    {
        public CpuException() : base()
        {
        }

        public CpuException(string message) : base(message)
        {
        }

        public CpuException(string message, Exception cause) : base(message, cause)
        {
        }

        public CpuException(Exception cause) : base(string.Empty, cause)
        {
        }
    }
}