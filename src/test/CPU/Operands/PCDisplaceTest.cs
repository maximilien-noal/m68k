namespace M68k.CPU.Operands
{
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
            model.MEM.Poke(codebase + 0x00e4, 0x41faff1c, Size.SizeLong);
            model.MEM.Poke(codebase, 0x00c00000, Size.SizeLong);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetPC(codebase + 0x00e4);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 2, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(model.CPU.XFlag));
            Assert.False(model.CPU.IsSet(model.CPU.NFlag));
            Assert.False(model.CPU.IsSet(model.CPU.ZFlag));
            Assert.False(model.CPU.IsSet(model.CPU.VFlag));
            Assert.False(model.CPU.IsSet(model.CPU.CFlag));
        }
    }
}