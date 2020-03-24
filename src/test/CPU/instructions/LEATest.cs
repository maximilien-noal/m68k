using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class LEATest : BasicSetup
    {
        public LEATest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstructionParamW(0x41f8, 0x1000);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x00001000, model.CPU.GetAddrRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}