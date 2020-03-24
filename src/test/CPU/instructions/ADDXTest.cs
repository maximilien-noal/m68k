using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Add
{
    public class ADDXTest : BasicSetup
    {
        public ADDXTest() : base()
        {
        }

        [Fact]
        public virtual void TestByte()
        {
            SetInstruction(0xd101);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0x78);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x123456F1, model.CPU.GetDataRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xd181);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, -126462926);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x0aaaaaab, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xd141);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, 0xaa78);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x123400F1, model.CPU.GetDataRegister(0));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}