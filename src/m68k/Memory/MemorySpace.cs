namespace M68k.Memory
{
    using System;
    using System.Buffers.Binary;
    using System.IO;

    public class MemorySpace : IAddressSpace, IDisposable
    {
        private readonly int size;

        private bool disposedValue;

        private MemoryStream memStream;

        public MemorySpace(int sizeKb)
        {
            size = sizeKb * 1024;
            memStream = new MemoryStream(new byte[size], 0, size, true, false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int GetEndAddress()
        {
            return size;
        }

        public int GetStartAddress()
        {
            return 0;
        }

        public byte InternalReadByte(int addr)
        {
            return ReadByte(addr);
        }

        public uint InternalReadLong(int addr)
        {
            return ReadLong(addr);
        }

        public int InternalReadWord(int addr)
        {
            return ReadWord(addr);
        }

        public void InternalWriteByte(int addr, int value)
        {
            WriteByte(addr, value);
        }

        public void InternalWriteLong(int addr, uint value)
        {
            WriteLong(addr, value);
        }

        public void InternalWriteWord(int addr, int value)
        {
            WriteWord(addr, value);
        }

        public byte ReadByte(int addr)
        {
            memStream.Position = addr;
            var value = memStream.ReadByte();
            return (byte)(value & 0x00ff);
        }

        public uint ReadLong(int addr)
        {
            var buffer = new byte[4];
            memStream.Position = addr;
            memStream.Read(buffer, 0, buffer.Length);
            var value = BinaryPrimitives.ReadUInt32BigEndian(buffer);
            return value;
        }

        public int ReadWord(int addr)
        {
            var buffer = new byte[2];
            memStream.Position = addr;
            memStream.Read(buffer, 0, buffer.Length);
            var value = BinaryPrimitives.ReadInt16BigEndian(buffer);
            var fullValue = value & 0x0000ffff;
            return fullValue;
        }

        public void Reset()
        {
            memStream.Close();
            memStream.Dispose();
            memStream = new MemoryStream(new byte[size], 0, size, true, false);
        }

        public int Size()
        {
            return size;
        }

        public void WriteByte(int addr, int value)
        {
            memStream.Position = addr;
            var fullValue = (byte)(value & 0x00ff);
            memStream.WriteByte(fullValue);
        }

        public virtual void WriteLong(int addr, uint value)
        {
            memStream.Position = addr;
            Span<byte> destination = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(destination, value);
            memStream.Write(destination.ToArray(), 0, destination.Length);
        }

        public virtual void WriteWord(int addr, int value)
        {
            memStream.Position = addr;
            short fullValue = (short)(value & 0x0000ffff);
            Span<byte> destination = stackalloc byte[2];
            BinaryPrimitives.WriteInt16BigEndian(destination, fullValue);
            memStream.Write(destination.ToArray(), 0, destination.Length);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.memStream.Close();
                    this.memStream.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}