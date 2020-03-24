using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class RTSTest : BasicSetup
    {
        public RTSTest() : base()
        {
        }

        [Fact]
        public virtual void TestReturn()
        {
            SetInstruction(0x4e75);
            model.CPU.SetCCR((byte)0);
            model.CPU.Push(codebase + 100, Size.SizeLong);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 100, model.CPU.GetPC());
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}