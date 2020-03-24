using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVE_2_CCRTest : BasicSetup
    {
        public MOVE_2_CCRTest() : base()
        {
        }

        [Fact]
        public virtual void TestMove()
        {
            SetInstruction(0x44c0);
            model.CPU.SetDataRegister(0, 0x0015);
            model.CPU.SetSR(0xff00);
            _ = model.CPU.Execute();
            Assert.Equal(0xff15, model.CPU.GetSR());
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}