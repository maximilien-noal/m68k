using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Tst
{
    public class TSTTest : BasicSetup
    {
        public TSTTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x4a00);
            model.CPU.SetDataRegister(0, 0x12345600);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345600, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x4a80);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406815, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x4a40);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(305419896, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}