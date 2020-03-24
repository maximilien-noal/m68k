using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Add
{
    public class ADDQTest : BasicSetup
    {
        public ADDQTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x5e00);
            model.CPU.SetDataRegister(0, 0x123456fc);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345603, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x5e80);
            model.CPU.SetDataRegister(0, 0x1234fffc);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12350003, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x5e40);
            model.CPU.SetDataRegister(0, 0x1234fffc);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12340003, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}