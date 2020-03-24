using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class CMPATest : BasicSetup
    {
        public CMPATest() : base()
        {
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xb1c9);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetAddrRegister(1, -872363008);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0xb2d0);
            model.CPU.SetAddrRegister(0, 32);
            model.CPU.SetAddrRegister(1, -2023406815);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xb0c9);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetAddrRegister(1, -2023390431);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}