using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVETest : BasicSetup
    {
        public MOVETest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0x1001);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetDataRegister(1, -943208505);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406649, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0x2001);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetDataRegister(1, -943208505);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-943208505, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestMem()
        {
            SetInstruction(0x22d8);
            model.CPU.SetAddrRegister(0, 32);
            model.CPU.SetAddrRegister(1, 40);
            model.MEM.Poke(32, -2023406815, Size.SizeLong);
            model.MEM.Poke(40, 305419896, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023406815, model.MEM.Peek(40, Size.SizeLong));
            Assert.Equal(36, model.CPU.GetAddrRegister(0));
            Assert.Equal(44, model.CPU.GetAddrRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0x3001);
            model.CPU.SetDataRegister(0, -2023406815);
            model.CPU.SetDataRegister(1, -943208505);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(-2023372857, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}