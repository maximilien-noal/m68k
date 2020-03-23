namespace test.CPU.Operands
{
    using M68k.CPU;

    using Miggy;

    using Xunit;

    public class PCDisplaceTest : BasicSetup
    {
        public PCDisplaceTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SystemModel.MEM.Poke(codebase + 0x00e4, 0x41faff1c, Size.SizeLong);
            SystemModel.MEM.Poke(codebase, 0x00c00000, Size.SizeLong);
            SystemModel.CPU.SetAddrRegister(0, -2023406815);
            SystemModel.CPU.SetPC(codebase + 0x00e4);
            SystemModel.CPU.SetCCR((byte)0);
            _ = SystemModel.CPU.Execute();
            Assert.Equal(codebase + 2, SystemModel.CPU.GetAddrRegister(0));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.XFlag));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.NFlag));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.ZFlag));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.VFlag));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.CFlag));
        }
    }
}