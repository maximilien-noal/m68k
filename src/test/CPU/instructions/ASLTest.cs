using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class ASLTest : BasicSetup
    {
        public ASLTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xe700);
            model.CPU.SetDataRegister(0, 0x567801);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x567808, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xe7a0);
            model.CPU.SetDataRegister(0, 0x567801);
            model.CPU.SetDataRegister(3, 20);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2146435072, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0xe1d0);
            model.CPU.SetAddrRegister(0, 32);
            model.MEM.Poke(32, 305419896, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x24685678, model.MEM.Peek(32, Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xe760);
            model.CPU.SetDataRegister(0, 0x567801);
            model.CPU.SetDataRegister(3, 14);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x564000, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}