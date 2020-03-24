namespace M68k.CPU.Instructions
{
    using Miggy;

    using Xunit;

    public class BxxTest : BasicSetup
    {
        public BxxTest() : base()
        {
        }

        [Fact]
        public virtual void TestBCC_Branch()
        {
            model.MEM.Poke(codebase + 6, 0x64f8, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestBCC_Branch_w()
        {
            model.MEM.Poke(codebase + 6, 0x6400, Size.Word);
            model.MEM.Poke(codebase + 8, 0xfff8, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestBCS_NoBranch()
        {
            model.MEM.Poke(codebase + 6, 0x65f8, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 8, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestBCS_NoBranch_w()
        {
            model.MEM.Poke(codebase + 6, 0x6500, Size.Word);
            model.MEM.Poke(codebase + 8, 0xfff8, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 10, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestBRA()
        {
            model.MEM.Poke(codebase + 6, 0x6000, Size.Word);
            model.MEM.Poke(codebase + 8, 0x0004, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 0x0c, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestBSR()
        {
            model.MEM.Poke(codebase + 6, 0x6100, Size.Word);
            model.MEM.Poke(codebase + 8, 0x0004, Size.Word);
            model.CPU.SetPC(codebase + 6);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 0x0c, model.CPU.GetPC());
            Assert.Equal(0x7fc, model.CPU.GetAddrRegister(7));
            Assert.Equal(codebase + 0x0a, model.MEM.Peek(0x7fc, Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}