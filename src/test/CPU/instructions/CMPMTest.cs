namespace M68k.CPU.Instructions
{
    using Miggy;

    using Xunit;

    public class CMPMTest : BasicSetup
    {
        public CMPMTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xb308);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetAddrRegister(1, 40);
            model.MEM.Poke(40, -2023406815, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(33, model.CPU.GetAddrRegister(0));
            Assert.Equal(41, model.CPU.GetAddrRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xb388);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetAddrRegister(1, 40);
            model.MEM.Poke(40, -872363008, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(36, model.CPU.GetAddrRegister(0));
            Assert.Equal(44, model.CPU.GetAddrRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xb348);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetAddrRegister(1, 40);
            model.MEM.Poke(40, 305419896, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(34, model.CPU.GetAddrRegister(0));
            Assert.Equal(42, model.CPU.GetAddrRegister(1));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}