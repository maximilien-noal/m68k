using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.BitShift
{
    public class ROXLTest : BasicSetup
    {
        public ROXLTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xe330);
            model.CPU.SetDataRegister(0, -2023406616);
            model.CPU.SetDataRegister(1, 4);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406713, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xe3b0);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetDataRegister(1, 18);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0c850eca, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0xe5d0);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x0eca4321, model.MEM.Peek(32, Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xe150);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023415519, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}