using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Neg
{
    public class NEGTest : BasicSetup
    {
        public NEGTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x4400);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406625, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x4480);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x789abcdf, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x4440);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023375649, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}