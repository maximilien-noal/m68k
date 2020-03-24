using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class MULSTest : BasicSetup
    {
        public MULSTest() : base()
        {
        }

        [Fact]
        public virtual void TestNeg()
        {
            SetInstruction(0xc1c1);
            model.CPU.SetDataRegister(0, -30875);
            model.CPU.SetDataRegister(1, 0x0033);
            _ = model.CPU.Execute();
            Assert.Equal(-1574625, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestPos()
        {
            SetInstruction(0xc1c1);
            model.CPU.SetDataRegister(0, 0x7765);
            model.CPU.SetDataRegister(1, 0x0345);
            _ = model.CPU.Execute();
            Assert.Equal(0x01865d39, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}