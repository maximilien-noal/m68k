namespace M68k.CPU.Instructions
{
    using M68k.CPU;

    using Miggy;

    using Xunit;

    public class CHKTest : BasicSetup
    {
        public CHKTest() : base()
        {
        }

        [Fact]
        public virtual void TestGreater()
        {
            SetInstruction(0x4181);
            model.CPU.SetDataRegister(0, 0x6321);
            model.CPU.SetDataRegister(1, 0x5678);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(6, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestNeg()
        {
            SetInstruction(0x4181);
            model.CPU.SetDataRegister(0, 0xc321);
            model.CPU.SetDataRegister(1, 0x5678);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.True(model.CPU.IsSupervisorMode());
            Assert.Equal(6, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestNoException()
        {
            SetInstruction(0x4181);
            model.CPU.SetDataRegister(0, 0x4321);
            model.CPU.SetDataRegister(1, 0x5678);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.False(model.CPU.IsSupervisorMode());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}