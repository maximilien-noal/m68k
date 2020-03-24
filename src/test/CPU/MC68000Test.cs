namespace M68k.CPU
{
    using M68k.Memory;

    using Xunit;

    public class MC68000Test
    {
        private readonly IAddressSpace bus;

        private readonly ICPU cpu;

        public MC68000Test()
        {
            bus = new MemorySpace(1);
            cpu = new MC68000();
            cpu.SetAddressSpace(bus);
            cpu.Reset();
        }

        [Fact]
        public virtual void TestAddrRegs()
        {
            for (int r = 0; r < 7; r++)
            {
                cpu.SetAddrRegisterByte(r, 0xaa);
                Assert.Equal(0xaa, cpu.GetAddrRegisterByte(r));
                Assert.Equal(0xaa, cpu.GetAddrRegisterWord(r));
                Assert.Equal(0xaa, cpu.GetAddrRegisterLong(r));
                Assert.Equal(-86, cpu.GetAddrRegisterByteSigned(r));
                Assert.Equal(170, cpu.GetAddrRegisterWordSigned(r));
                cpu.SetAddrRegisterWord(r, 0xa5a5);
                Assert.Equal(0xa5, cpu.GetAddrRegisterByte(r));
                Assert.Equal(0xa5a5, cpu.GetAddrRegisterWord(r));
                Assert.Equal(0xa5a5, cpu.GetAddrRegisterLong(r));
                Assert.Equal(-91, cpu.GetAddrRegisterByteSigned(r));
                Assert.Equal(-23131, cpu.GetAddrRegisterWordSigned(r));
                cpu.SetAddrRegisterLong(r, -2054847099);
                Assert.Equal(0x85, cpu.GetAddrRegisterByte(r));
                Assert.Equal(0x8585, cpu.GetAddrRegisterWord(r));
                Assert.Equal(-2054847099, cpu.GetAddrRegisterLong(r));
                Assert.Equal(-123, cpu.GetAddrRegisterByteSigned(r));
                Assert.Equal(-31355, cpu.GetAddrRegisterWordSigned(r));
                cpu.SetAddrRegisterLong(r, 305419896);
                Assert.Equal(0x78, cpu.GetAddrRegisterByte(r));
                Assert.Equal(0x5678, cpu.GetAddrRegisterWord(r));
                Assert.Equal(305419896, cpu.GetAddrRegisterLong(r));
                Assert.Equal(0x78, cpu.GetAddrRegisterByteSigned(r));
                Assert.Equal(0x5678, cpu.GetAddrRegisterWordSigned(r));
            }
        }

        [Fact]
        public virtual void TestDataRegs()
        {
            for (int r = 0; r < 7; r++)
            {
                cpu.SetDataRegisterByte(r, 0xaa);
                Assert.Equal(0xaa, cpu.GetDataRegisterByte(r));
                Assert.Equal(0xaa, cpu.GetDataRegisterWord(r));
                Assert.Equal(0xaa, cpu.GetDataRegisterLong(r));
                Assert.Equal(-86, cpu.GetDataRegisterByteSigned(r));
                Assert.Equal(170, cpu.GetDataRegisterWordSigned(r));
                cpu.SetDataRegisterWord(r, 0xa5a5);
                Assert.Equal(0xa5, cpu.GetDataRegisterByte(r));
                Assert.Equal(0xa5a5, cpu.GetDataRegisterWord(r));
                Assert.Equal(0xa5a5, cpu.GetDataRegisterLong(r));
                Assert.Equal(-91, cpu.GetDataRegisterByteSigned(r));
                Assert.Equal(-23131, cpu.GetDataRegisterWordSigned(r));
                cpu.SetDataRegisterLong(r, -2054847099);
                Assert.Equal(0x85, cpu.GetDataRegisterByte(r));
                Assert.Equal(0x8585, cpu.GetDataRegisterWord(r));
                Assert.Equal(-2054847099, cpu.GetDataRegisterLong(r));
                Assert.Equal(-123, cpu.GetDataRegisterByteSigned(r));
                Assert.Equal(-31355, cpu.GetDataRegisterWordSigned(r));
                cpu.SetDataRegisterLong(r, 305419896);
                Assert.Equal(0x78, cpu.GetDataRegisterByte(r));
                Assert.Equal(0x5678, cpu.GetDataRegisterWord(r));
                Assert.Equal(305419896, cpu.GetDataRegisterLong(r));
                Assert.Equal(0x78, cpu.GetDataRegisterByteSigned(r));
                Assert.Equal(0x5678, cpu.GetDataRegisterWordSigned(r));
            }
        }

        [Fact]
        public virtual void TestException()
        {
            bus.WriteLong(0x08, 0x56789);
            bus.WriteLong(0x0c, 0x12345);
            bus.WriteLong(0x10, 0x23456);
            cpu.SetPC(0x32);
            cpu.SetSR(0x04);
            cpu.SetAddrRegisterLong(7, 0x0100);
            cpu.SetSSP(0x0200);
            cpu.RaiseException(2);
            Assert.Equal(0x56789, cpu.GetPC());
            Assert.Equal(0x01fa, cpu.GetAddrRegisterLong(7));
            Assert.Equal(0x0100, cpu.GetUSP());
            Assert.True(cpu.IsSupervisorMode());
            int sr = cpu.PopWord();
            int pc = cpu.PopLong();
            Assert.Equal(0x04, sr);
            Assert.Equal(0x32, pc);
            cpu.SetSR(sr);
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0x0100, cpu.GetUSP());
            Assert.Equal(0x0100, cpu.GetAddrRegisterLong(7));
            Assert.Equal(0x0200, cpu.GetSSP());
        }

        [Fact]
        public virtual void TestFlags()
        {
            cpu.SetSR(0x27ff);
            Assert.True(cpu.IsFlagSet(cpu.CFlag));
            Assert.True(cpu.IsFlagSet(cpu.VFlag));
            Assert.True(cpu.IsFlagSet(cpu.ZFlag));
            Assert.True(cpu.IsFlagSet(cpu.NFlag));
            Assert.True(cpu.IsFlagSet(cpu.XFlag));
            Assert.True(cpu.IsSupervisorMode());
            Assert.Equal(7, cpu.GetInterruptLevel());
            cpu.SetSR(0);
            Assert.False(cpu.IsFlagSet(cpu.CFlag));
            Assert.False(cpu.IsFlagSet(cpu.VFlag));
            Assert.False(cpu.IsFlagSet(cpu.ZFlag));
            Assert.False(cpu.IsFlagSet(cpu.NFlag));
            Assert.False(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
            cpu.SetCCRegister(cpu.CFlag);
            Assert.True(cpu.IsFlagSet(cpu.CFlag));
            Assert.False(cpu.IsFlagSet(cpu.VFlag));
            Assert.False(cpu.IsFlagSet(cpu.ZFlag));
            Assert.False(cpu.IsFlagSet(cpu.NFlag));
            Assert.False(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
            cpu.SetCCRegister(cpu.VFlag);
            Assert.False(cpu.IsFlagSet(cpu.CFlag));
            Assert.True(cpu.IsFlagSet(cpu.VFlag));
            Assert.False(cpu.IsFlagSet(cpu.ZFlag));
            Assert.False(cpu.IsFlagSet(cpu.NFlag));
            Assert.False(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
            cpu.SetCCRegister(cpu.ZFlag);
            Assert.False(cpu.IsFlagSet(cpu.CFlag));
            Assert.False(cpu.IsFlagSet(cpu.VFlag));
            Assert.True(cpu.IsFlagSet(cpu.ZFlag));
            Assert.False(cpu.IsFlagSet(cpu.NFlag));
            Assert.False(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
            cpu.SetCCRegister(cpu.NFlag);
            Assert.False(cpu.IsFlagSet(cpu.CFlag));
            Assert.False(cpu.IsFlagSet(cpu.VFlag));
            Assert.False(cpu.IsFlagSet(cpu.ZFlag));
            Assert.True(cpu.IsFlagSet(cpu.NFlag));
            Assert.False(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
            cpu.SetCCRegister(cpu.XFlag);
            Assert.False(cpu.IsFlagSet(cpu.CFlag));
            Assert.False(cpu.IsFlagSet(cpu.VFlag));
            Assert.False(cpu.IsFlagSet(cpu.ZFlag));
            Assert.False(cpu.IsFlagSet(cpu.NFlag));
            Assert.True(cpu.IsFlagSet(cpu.XFlag));
            Assert.False(cpu.IsSupervisorMode());
            Assert.Equal(0, cpu.GetInterruptLevel());
        }

        [Fact]
        public virtual void TestPC()
        {
            bus.WriteLong(4, 305419896);
            cpu.SetPC(4);
            int val = cpu.FetchPCLong();
            Assert.Equal(305419896, val);
            Assert.Equal(8, cpu.GetPC());
            cpu.SetPC(4);
            val = cpu.FetchPCWord();
            Assert.Equal(0x1234, val);
            Assert.Equal(6, cpu.GetPC());
            val = cpu.FetchPCWord();
            Assert.Equal(0x5678, val);
            Assert.Equal(8, cpu.GetPC());
            cpu.SetPC(4);
            bus.WriteLong(4, 0x12348765);
            val = cpu.FetchPCWordSigned();
            Assert.Equal(0x1234, val);
            Assert.Equal(6, cpu.GetPC());
            val = cpu.FetchPCWordSigned();
            Assert.Equal(-30875, val);
            Assert.Equal(8, cpu.GetPC());
        }
    }
}