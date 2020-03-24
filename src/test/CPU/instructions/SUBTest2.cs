using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Sub
{
    public class SUBTest2 : BasicSetup
    {
        public SUBTest2() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x9001);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0x78);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345600, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x9081);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-1966140585, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x9041);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0xaa78);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x1234ac00, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}