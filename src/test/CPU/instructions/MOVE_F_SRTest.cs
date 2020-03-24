using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVE_F_SRTest : BasicSetup
    {
        public MOVE_F_SRTest() : base()
        {
        }

        [Fact]
        public virtual void TestMove()
        {
            SetInstruction(0x40c0);
            model.CPU.SetSR((short)0x000f);
            _ = model.CPU.Execute();
            Assert.Equal((short)0x000f, model.CPU.GetSR());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}