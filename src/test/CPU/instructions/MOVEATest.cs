using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVEATest : BasicSetup
    {
        public MOVEATest() : base()
        {
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x2040);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetAddrRegister(0, -943208505);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406815, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x3040);
            model.CPU.SetDataRegister(0, 0x8004);
            model.CPU.SetAddrRegister(0, -943208505);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-32764, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}