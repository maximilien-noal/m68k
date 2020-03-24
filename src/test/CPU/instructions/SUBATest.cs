using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Sub
{
    public class SUBATest : BasicSetup
    {
        public SUBATest() : base()
        {
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x91c0);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x7530eca9, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x90c0);
            model.CPU.SetDataRegister(0, 0xc678);
            model.CPU.SetAddrRegister(0, 0x00ff7800);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x00ffb188, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}