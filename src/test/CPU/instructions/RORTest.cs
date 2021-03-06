using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.BitShift
{
    public class RORTest : BasicSetup
    {
        public RORTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xe238);
            model.CPU.SetDataRegister(0, -2023406616);
            model.CPU.SetDataRegister(1, 4);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406706, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xe2b8);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetDataRegister(1, 18);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(1355309529, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0xe6d0);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-1011727583, model.MEM.Peek(32, Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xe058);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023415485, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}