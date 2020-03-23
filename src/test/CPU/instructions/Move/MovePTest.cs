namespace M68k.CPU.Instructions.Move
{
    using Miggy;

    using Xunit;

    public class MovePTest : BasicSetup
    {
        [Fact]
        public virtual void TestLongToMem()
        {
            SetInstructionParamW(0x01c8, 0x0000);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetAddrRegister(0, codebase + 101);
            model.MEM.Poke(codebase + 100, -1, Size.SizeLong);
            model.MEM.Poke(codebase + 104, -1, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            int val = model.MEM.Peek(codebase + 100, Size.SizeLong);
            Assert.Equal(-7864475, val);
            val = model.MEM.Peek(codebase + 104, Size.SizeLong);
            Assert.Equal(-12320991, val);
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLongToReg()
        {
            SetInstructionParamW(0x0348, 0x0000);
            model.CPU.SetAddrRegister(0, codebase + 101);
            model.CPU.SetDataRegister(1, -1061109568);
            model.MEM.Poke(codebase + 100, -7864475, Size.SizeLong);
            model.MEM.Poke(codebase + 104, -12320991, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406815, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWordToMem()
        {
            SetInstructionParamW(0x0188, 0x0000);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetAddrRegister(0, codebase + 100);
            model.MEM.Poke(codebase + 100, -1, Size.SizeLong);
            model.MEM.Poke(codebase + 104, -1, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            int val = model.MEM.Peek(codebase + 100, Size.SizeLong);
            Assert.Equal(1140793855, val);
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWordToReg()
        {
            SetInstructionParamW(0x0308, 0x0000);
            model.CPU.SetAddrRegister(0, codebase + 100);
            model.CPU.SetDataRegister(1, -1061109568);
            model.MEM.Poke(codebase + 100, 0x43ff21ff, Size.SizeLong);
            model.MEM.Poke(codebase + 104, -1, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-1061141727, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}