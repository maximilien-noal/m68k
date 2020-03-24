using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.BitShift
{
    public class SBCDTest : BasicSetup
    {
        public SBCDTest() : base()
        {
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0x8109);
            model.CPU.SetAddrRegister(0, codebase + 100);
            model.CPU.SetAddrRegister(1, codebase + 108);
            model.MEM.Poke(codebase + 98, 0x0100, Size.Word);
            model.MEM.Poke(codebase + 106, 0x0001, Size.Word);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0199, model.MEM.Peek(codebase + 98, Size.Word));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            model.CPU.SetPC(codebase);
            _ = model.CPU.Execute();
            Assert.Equal(0x0099, model.MEM.Peek(codebase + 98, Size.Word));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestReg()
        {
            SetInstruction(0x8101);
            model.CPU.SetDataRegister(0, 0x0099);
            model.CPU.SetDataRegister(1, 0x0001);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0098, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}