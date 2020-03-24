using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class ILLEGALTest : BasicSetup
    {
        public ILLEGALTest() : base()
        {
        }

        [Fact]
        public virtual void TestTrap()
        {
            SetInstruction(0x4afc);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(4, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}