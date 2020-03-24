using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class DIVUTest : BasicSetup
    {
        public DIVUTest() : base()
        {
        }

        [Fact]
        public virtual void TestDivByZero()
        {
            SetInstruction(0x80c1);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(5, model.CPU.GetPC());
            Assert.Equal(305419896, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestOverflow()
        {
            SetInstruction(0x80c1);
            model.CPU.SetDataRegister(0, -30875);
            model.CPU.SetDataRegister(1, 0x0003);
            _ = model.CPU.Execute();
            Assert.Equal(-30875, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestPos()
        {
            SetInstruction(0x80c1);
            model.CPU.SetDataRegister(0, 0x8765);
            model.CPU.SetDataRegister(1, 0x0003);
            _ = model.CPU.Execute();
            Assert.Equal(0x00022d21, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}