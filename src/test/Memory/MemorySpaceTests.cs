namespace m68k.memory
{
    using M68k.Memory;

    using Xunit;

    public class MemorySpaceTests
    {
        [Fact]
        public void TestCreate()
        {
            var bus = new MemorySpace(4);
            Assert.Equal(4096, bus.Size());

            bus = new MemorySpace(32);
            Assert.Equal(32768, bus.Size());
        }

        [Fact]
        public void TestMemory()
        {
            using var bus = new MemorySpace(1);
            bus.WriteByte(4, 0x55);
            Assert.Equal((uint)0x55000000, bus.ReadLong(4));
            Assert.Equal(0x5500, bus.ReadWord(4));
            Assert.Equal(0x55, bus.ReadByte(4));

            bus.WriteWord(4, 0x1234);
            Assert.Equal((uint)0x12340000, bus.ReadLong(4));
            Assert.Equal(0x1234, bus.ReadWord(4));
            Assert.Equal(0x12, bus.ReadByte(4));
            Assert.Equal(0x34, bus.ReadByte(5));

            bus.WriteLong(4, 0x98765432);
            Assert.Equal(0x98765432, bus.ReadLong(4));
            Assert.Equal(0x9876, bus.ReadWord(4));
            Assert.Equal(0x5432, bus.ReadWord(6));
            Assert.Equal(0x98, bus.ReadByte(4));
            Assert.Equal(0x76, bus.ReadByte(5));
            Assert.Equal(0x54, bus.ReadByte(6));
            Assert.Equal(0x32, bus.ReadByte(7));
        }
    }
}