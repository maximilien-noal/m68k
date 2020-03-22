namespace M68k.Cpu.Instructions
{
    using M68k.CPU;
    using M68k.CPU.Instructions;
    using M68k.Memory;

    using Xunit;

    public class TasTest
    {
        internal IAddressSpace bus;

        internal MC68000 cpu;

        public TasTest()
        {
            //create 1kb of memory for the cpu
            bus = new MemorySpace(1);

            cpu = new MC68000();
            cpu.AddressSpace = bus;
            cpu.Reset();
            cpu.SetAddrRegisterLong(7, 0x200);
        }

        [Fact]
        public void TestTasBroken()
        {
            TAS.EmulateBrokenTAS = true;
            bus.WriteWord(4, 0x4AC0); //TAS D0
            cpu.SetPC(4);
            cpu.SetDataRegisterLong(0, 0);

            cpu.Execute();
            int res = cpu.GetDataRegisterLong(0);
            Assert.Equal(0, (res & 0xFF) >> 7);
        }

        // 0100 1010 1100 0000
        [Fact]
        public void TestTasOk()
        {
            TAS.EmulateBrokenTAS = false;
            bus.WriteWord(4, 0x4AC0); //TAS D0
            cpu.SetPC(4);
            cpu.SetDataRegisterLong(0, 0);

            cpu.Execute();
            int res = cpu.GetDataRegisterLong(0);
            Assert.Equal(1, (res & 0xFF) >> 7);
        }
    }
}