namespace M68k.CPU.Instructions
{
    using Miggy;

    using Xunit;

    public class UNLKTest : BasicSetup
    {
        public UNLKTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstruction(0x4e5e);
            int stack = model.CPU.GetAddrRegister(7);
            model.CPU.Push(-2023406815, Size.SizeLong);
            model.CPU.SetAddrRegister(6, stack - 4);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(stack, model.CPU.GetAddrRegister(7));
            Assert.Equal(-2023406815, model.CPU.GetAddrRegister(6));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}