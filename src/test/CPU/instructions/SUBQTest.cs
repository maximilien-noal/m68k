using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Sub
{
    public class SUBQTest : BasicSetup
    {
        public SUBQTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x5100);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345670, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x5d80);
            model.CPU.SetDataRegister(0, -2147483645);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x7ffffffd, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x5f40);
            model.CPU.SetDataRegister(0, 0x12340002);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x1234fffb, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}