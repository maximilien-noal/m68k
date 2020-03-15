using System;
using System.IO;

namespace M68k.Memory
{
    public sealed class MemorySpace : IAddressSpace
    {
        private readonly MemoryStream memStream;

        private readonly uint size;

        // To detect redundant calls
        private bool disposedValue = false;

        public MemorySpace(uint sizeKb)
        {
            size = sizeKb * 1024;
            memStream = new MemoryStream((int)size);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        public uint GetEndAddress()
        {
            return size;
        }

        public uint GetStartAddress()
        {
            return 0;
        }

        public uint InternalReadByte(uint addr)
        {
            return ReadByte(addr);
        }

        public uint InternalReadLong(uint addr)
        {
            return ReadLong(addr);
        }

        public uint InternalReadWord(uint addr)
        {
            return ReadWord(addr);
        }

        public void InternalWriteByte(uint addr, uint value)
        {
            WriteByte(addr, value);
        }

        public void InternalWriteLong(uint addr, uint value)
        {
            WriteLong(addr, value);
        }

        public void InternalWriteWord(uint addr, uint value)
        {
            WriteWord(addr, value);
        }

        public uint ReadByte(uint addr)
        {
            var oldPosition = memStream.Position;
            memStream.Position = addr;
            uint v = (uint)memStream.ReadByte();
            memStream.Position = oldPosition;
            return v & 0x00ff;
        }

        public uint ReadLong(uint addr)
        {
            var buffer = new byte[4];
            memStream.Read(buffer, (int)addr, buffer.Length);
            return (uint)BitConverter.ToInt32(buffer, (int)addr);
        }

        public uint ReadWord(uint addr)
        {
            var buffer = new byte[2];
            memStream.Read(buffer, (int)addr, buffer.Length);
            var v = (uint)BitConverter.ToInt16(buffer, (int)addr);
            return v & 0x0000ffff;
        }

        public void Reset()
        {
            // Method intentionally left empty.
        }

        public uint Size()
        {
            return size;
        }

        public void WriteByte(uint addr, uint value)
        {
            var oldPosition = memStream.Position;
            memStream.Position = addr;
            memStream.WriteByte((byte)(value & 0x00ff));
            memStream.Position = oldPosition;
        }

        public void WriteLong(uint addr, uint value)
        {
            using (var writer = new BinaryWriter(memStream))
            {
                writer.Seek((int)addr, SeekOrigin.Begin);
                writer.Write(value);
            }
        }

        public void WriteWord(uint addr, uint value)
        {
            using (var writer = new BinaryWriter(memStream))
            {
                writer.Seek((int)addr, SeekOrigin.Begin);
                writer.Write((short)(value & 0x0000ffff));
            }
        }

        private void Dispose(bool disposing)
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