namespace M68k.CPU
{
    public struct Size : System.IEquatable<Size>
    {
        public const string BYTESIZE = ".b";

        public const string LONGSIZE = ".l";

        public const string UNSIZED = "";

        public const string WORDSIZE = ".w";

        public static readonly Size Byte = new Size(1, BYTESIZE, 0x80);

        public static readonly Size SizeLong = new Size(4, LONGSIZE, 0x7FFFFFFF);

        public static readonly Size Unsized = new Size(0, "", 0);

        public static readonly Size Word = new Size(2, WORDSIZE, 0x8000);

        private Size(int numBytes, string ext, int msb)
        {
            this.ByteCount = numBytes;
            this.Ext = ext;
            this.MSB = msb;
            this.Mask = (msb * 2) - 1;
        }

        public int ByteCount { get; }

        public string Ext { get; }

        public int Mask { get; }

        public int MSB { get; }

        public static bool operator !=(Size left, Size right)
        {
            return !(left == right);
        }

        public static bool operator ==(Size left, Size right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Size))
            {
                return false;
            }
            return Equals((Size)obj);
        }

        public bool Equals(Size other)
        {
            return other.ByteCount == this.ByteCount && other.Mask == this.Mask && other.MSB == this.MSB;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}