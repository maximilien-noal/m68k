using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class TRAPVTest : BasicSetup
    {
        public TRAPVTest() : base()
        {
        }

        [Fact]
        public virtual void TestNoTrap()
        {
            SetInstruction(0x4e76);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 2, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestTrap()
        {
            SetInstruction(0x4e76);
            model.CPU.SetCCR((byte)2);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(7, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}