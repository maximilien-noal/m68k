using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class RTRTest : BasicSetup
    {
        public RTRTest() : base()
        {
        }

        [Fact]
        public virtual void TestReturn()
        {
            SetInstruction(0x4e77);
            model.CPU.SetCCR((byte)0);
            model.CPU.Push(codebase + 100, Size.SizeLong);
            model.CPU.Push(0x341f, Size.Word);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 100, model.CPU.GetPC());
            Assert.Equal(0x001f, model.CPU.GetSR());
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}