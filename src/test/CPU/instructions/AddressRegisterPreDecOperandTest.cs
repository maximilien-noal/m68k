namespace m68k.cpu.instructions
{
    using M68k.CPU;
    using M68k.Memory;

    using System.Collections.Generic;
    using System.Linq;

    using Xunit;

    public class AddressRegisterPreDecOperandTest
    {
        internal IAddressSpace bus;

        internal MC68000 cpu;

        internal Dictionary<int, int> wordWrites = new Dictionary<int, int>();

        public AddressRegisterPreDecOperandTest()
        {
            //create 1kb of memory for the cpu
            bus = new MemorySpaceAnonymousInnerClass(this);

            cpu = new MC68000();
            cpu.AddressSpace = bus;
            cpu.Reset();
            cpu.SetAddrRegisterLong(7, 0x200);
            wordWrites.Clear();
        }

        [Fact]
        public void TestLswWrittenFirstMOVE()
        {
            int lsw = 0x2222;
            int msw = 0x1111;
            int value = msw << 16 | lsw;
            int firstWordPos = 0x100;
            int secondWordPos = 0x102;
            int thirdWordPos = 0x104;

            bus.WriteWord(4, 0x2d01); //2d01 move.l   d1,-(a6)
            cpu.SetPC(4);
            cpu.SetDataRegisterLong(1, value);
            cpu.SetAddrRegisterLong(6, thirdWordPos);

            wordWrites.Clear();
            cpu.Execute();

            Assert.Equal(cpu.GetAddrRegisterLong(6), firstWordPos);

            long res = bus.ReadLong(firstWordPos);
            Assert.Equal(res, value);

            Assert.Equal(2, wordWrites.Count);

            KeyValuePair<int, int> first = wordWrites.FirstOrDefault();

            Assert.Equal(secondWordPos, first.Key);
            Assert.Equal(lsw, first.Value);
            KeyValuePair<int, int> second = wordWrites.ElementAtOrDefault(1);
            Assert.Equal(firstWordPos, second.Key);
            Assert.Equal(msw, second.Value);
        }

        [Fact]
        public void TestLswWrittenFirstMOVEM()
        {
            int lsw = 0x2222;
            int msw = 0x1111;
            int value = msw << 16 | lsw;
            int startPos = 0x104;
            int endPos = startPos - 8; // 2 longs
            int valuePos = startPos - 4;

            bus.WriteLong(4, 0x48e1_8100); //48e1 8100                movem.l  d0/d7,-(a1)
            cpu.SetPC(4);
            cpu.SetDataRegisterLong(7, value);
            cpu.SetAddrRegisterLong(1, startPos);

            wordWrites.Clear();
            cpu.Execute();

            Assert.Equal(cpu.GetAddrRegisterLong(1), endPos);

            long res = bus.ReadLong(valuePos);
            Assert.Equal(res, value);

            Assert.Equal(4, wordWrites.Count);

            int firstWordPos = startPos - 2;
            KeyValuePair<int, int> first = wordWrites.FirstOrDefault();
            Assert.Equal(firstWordPos, first.Key);
            Assert.Equal(lsw, first.Value);

            int secondWordPos = firstWordPos - 2;
            KeyValuePair<int, int> second = wordWrites.ElementAtOrDefault(1);
            Assert.Equal(secondWordPos, second.Key);
            Assert.Equal(msw, second.Value);
        }

        private class MemorySpaceAnonymousInnerClass : MemorySpace
        {
            private readonly AddressRegisterPreDecOperandTest outerInstance;

            public MemorySpaceAnonymousInnerClass(AddressRegisterPreDecOperandTest outerInstance) : base(1)
            {
                this.outerInstance = outerInstance;
            }

            public override void WriteLong(int addr, uint value)
            {
                outerInstance.wordWrites[addr] = (int)((value >> 16) & 0xFFFF);
                outerInstance.wordWrites[addr + 2] = (int)(value & 0xFFFF);
                base.WriteLong(addr, value);
            }

            public override void WriteWord(int addr, int value)
            {
                outerInstance.wordWrites[addr] = value;
                base.WriteWord(addr, value);
            }
        }
    }
}