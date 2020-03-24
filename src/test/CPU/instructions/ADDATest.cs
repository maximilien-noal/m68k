using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Add
{
    public class ADDATest : BasicSetup
    {
        public ADDATest() : base()
        {
        }

        [Fact]
        public virtual void TestLong()
        {
            SetInstruction(0xd1c0);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetAddrRegister(0, 0x56785678);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x68acacf0, model.CPU.GetAddrRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }

        [Fact]
        public virtual void TestWord()
        {
            SetInstruction(0xd0c0);
            model.CPU.SetDataRegister(0, 0xc234);
            model.CPU.SetAddrRegister(0, 0x56785678);
            model.CPU.SetCCR((byte)0x1f);
            _ = model.CPU.Execute();
            Assert.Equal(0x567818ac, model.CPU.GetAddrRegister(0));
            Assert.True(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.C_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.X_FLAG));
        }
    }
}