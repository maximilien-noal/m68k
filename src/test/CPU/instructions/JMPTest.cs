using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class JMPTest : BasicSetup
    {
        public JMPTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstructionParamL(0x4ed0, 0x00200000);
            model.CPU.SetAddrRegister(0, 2);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0002, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}