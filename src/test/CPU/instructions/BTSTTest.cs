using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class BTSTTest : BasicSetup
    {
        public BTSTTest() : base()
        {
        }

        [Fact]
        public virtual void TestDyn()
        {
            SetInstruction(0x0300);
            model.CPU.SetDataRegister(0, 0x0010);
            model.CPU.SetDataRegister(1, 4);
            model.CPU.SetCCR((byte)4);
            _ = model.CPU.Execute();
            Assert.Equal(0x0010, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestStatic()
        {
            SetInstructionParamW(0x0800, 0x001f);
            model.CPU.SetDataRegister(0, 0);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}