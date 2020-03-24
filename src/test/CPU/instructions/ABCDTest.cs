namespace M68k.CPU.Instructions.BitShift
{
    using Miggy;

    using Xunit;

    public class ABCDTest : BasicSetup
    {
        public ABCDTest() : base()
        {
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0xc109);
            model.CPU.SetAddrRegister(0, codebase + 100);
            model.CPU.SetAddrRegister(1, codebase + 108);
            model.MEM.Poke(codebase + 98, 0x0099, Size.Word);
            model.MEM.Poke(codebase + 106, 0x001, Size.Word);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0, model.MEM.Peek(codebase + 98, Size.Word));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            model.CPU.SetPC(codebase);
            _ = model.CPU.Execute();
            Assert.Equal(0x0100, model.MEM.Peek(codebase + 98, Size.Word));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestReg()
        {
            SetInstruction(0xc101);
            model.CPU.SetDataRegister(0, 0x0099);
            model.CPU.SetDataRegister(1, 0x0001);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}