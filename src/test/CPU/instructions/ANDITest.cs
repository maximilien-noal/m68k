using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.And
{
    public class ANDITest : BasicSetup
    {
        public ANDITest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstructionParamW(0x0200, 0x000f);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x12345608, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstructionParamL(0x0280, 1);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstructionParamW(0x0240, 0xf00f);
            model.CPU.SetDataRegister(0, 0x1234c678);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x1234c008, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}