using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVE_2_SRTest : BasicSetup
    {
        public MOVE_2_SRTest() : base()
        {
        }

        [Fact]
        public virtual void TestMove()
        {
            SetInstruction(0x46c0);
            model.CPU.SetDataRegister(0, 0x2015);
            model.CPU.SetSR((short)0x2000);
            _ = model.CPU.Execute();
            Assert.Equal((short)0x2015, model.CPU.GetSR());
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestMoveException()
        {
            SetInstruction(0x46c0);
            model.CPU.SetDataRegister(0, 0x2015);
            model.CPU.SetSR((short)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x2000, model.CPU.GetSR());
            Assert.Equal(8, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}