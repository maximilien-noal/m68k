using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.And
{
    public class ANDTest : BasicSetup
    {
        public ANDTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xc001);
            model.CPU.SetDataRegister(0, 0x12345688);
            model.CPU.SetDataRegister(1, 0x7f);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345608, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xc081);
            model.CPU.SetDataRegister(0, -2110499208);
            model.CPU.SetDataRegister(1, -126462926);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(-2144054224, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xc041);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0xaa78);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12340278, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}