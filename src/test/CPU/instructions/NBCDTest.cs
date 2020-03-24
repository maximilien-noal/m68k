using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.BitShift
{
    public class NBCDTest : BasicSetup
    {
        public NBCDTest() : base()
        {
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0x4820);
            model.CPU.SetAddrRegister(0, codebase + 100);
            model.MEM.Poke(codebase + 98, 0x0099, Size.Word);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0001, model.MEM.Peek(codebase + 98, Size.Word));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestReg()
        {
            SetInstruction(0x4800);
            model.CPU.SetDataRegister(0, 0x0099);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(1, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}