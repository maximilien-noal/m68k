using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class JSRTest : BasicSetup
    {
        public JSRTest() : base()
        {
        }

        [Fact]
        public virtual void TestInstruction()
        {
            SetInstruction(0x4e90);
            model.CPU.SetAddrRegister(0, codebase + 50);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(codebase + 50, model.CPU.GetPC());
            Assert.Equal(codebase + 2, model.MEM.Peek(model.CPU.GetAddrRegister(7), Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}