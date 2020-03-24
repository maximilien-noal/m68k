using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class EXGTest : BasicSetup
    {
        public EXGTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstruction(0xc141);
            model.CPU.SetDataRegister(0, -1737075662);
            model.CPU.SetDataRegister(1, 305419896);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(305419896, model.CPU.GetDataRegister(0));
            Assert.Equal(-1737075662, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}