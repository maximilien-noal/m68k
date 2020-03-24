using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class LINKTest : BasicSetup
    {
        public LINKTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstructionParamW(0x4e56, 0xfff8);
            model.CPU.SetAddrRegister(6, -2023406815);
            model.CPU.SetCCR((byte)0);
            int stack = model.CPU.GetAddrRegister(7);
            _ = model.CPU.Execute();
            Assert.Equal(stack - 12, model.CPU.GetAddrRegister(7));
            Assert.Equal(stack - 4, model.CPU.GetAddrRegister(6));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}