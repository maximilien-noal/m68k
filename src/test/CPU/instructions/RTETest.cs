using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class RTETest : BasicSetup
    {
        public RTETest() : base()
        {
        }

        [Fact]
        public virtual void TestPrivViolation()
        {
            SetInstruction(0x4e73);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(8, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestReturn()
        {
            SetInstruction(0x4e73);
            model.CPU.SetCCR((byte)0);
            model.CPU.SetSupervisorMode(true);
            model.CPU.Push(codebase + 100, Size.SizeLong);
            model.CPU.Push(0x001f, Size.Word);
            _ = model.CPU.Execute();
            Assert.False(model.CPU.IsSupervisorMode());
            Assert.Equal(codebase + 100, model.CPU.GetPC());
            Assert.Equal(0x001f, model.CPU.GetSR());
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}