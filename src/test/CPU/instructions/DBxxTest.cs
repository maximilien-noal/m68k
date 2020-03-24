using Miggy;

using Xunit;

namespace M68k.CPU.Instructions
{
    public class DBxxTest : BasicSetup
    {
        public DBxxTest() : base()
        {
        }

        [Fact]
        public virtual void TestDBEQ_Branch()
        {
            model.MEM.Poke(0, 0x7003, Size.Word);
            model.MEM.Poke(2, 0x7200, Size.Word);
            model.MEM.Poke(4, 0x5281, Size.Word);
            model.MEM.Poke(6, 0x0c01, Size.Word);
            model.MEM.Poke(8, 0x0003, Size.Word);
            model.MEM.Poke(10, 0x57c8, Size.Word);
            model.MEM.Poke(12, 0xfff8, Size.Word);
            model.MEM.Poke(14, 0x7000, Size.Word);
            model.CPU.SetDataRegister(0, 3);
            model.CPU.SetDataRegister(1, 1);
            model.CPU.SetPC(10);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(4, model.CPU.GetPC());
            Assert.Equal(2, model.CPU.GetDataRegister(0));
            Assert.Equal(1, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestDBEQ_Exit_Condition()
        {
            model.MEM.Poke(0, 0x7003, Size.Word);
            model.MEM.Poke(2, 0x7200, Size.Word);
            model.MEM.Poke(4, 0x5281, Size.Word);
            model.MEM.Poke(6, 0x0c01, Size.Word);
            model.MEM.Poke(8, 0x0003, Size.Word);
            model.MEM.Poke(10, 0x57c8, Size.Word);
            model.MEM.Poke(12, 0xfff8, Size.Word);
            model.MEM.Poke(14, 0x7000, Size.Word);
            model.CPU.SetDataRegister(0, 3);
            model.CPU.SetDataRegister(1, 1);
            model.CPU.SetPC(10);
            model.CPU.SetCCR((byte)4);
            _ = model.CPU.Execute();
            Assert.Equal(14, model.CPU.GetPC());
            Assert.Equal(3, model.CPU.GetDataRegister(0));
            Assert.Equal(1, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.True(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestDBEQ_Exit_Counter()
        {
            model.MEM.Poke(0, 0x7003, Size.Word);
            model.MEM.Poke(2, (short)0x7200, Size.Word);
            model.MEM.Poke(4, (short)0x5281, Size.Word);
            model.MEM.Poke(6, (short)0x0c01, Size.Word);
            model.MEM.Poke(8, (short)0x0003, Size.Word);
            model.MEM.Poke(10, (short)0x57c8, Size.Word);
            int x;
            unchecked
            {
                x = (short)0xfff8;
            }
            model.MEM.Poke(12, x, Size.Word);
            model.MEM.Poke(14, (short)0x7000, Size.Word);
            model.CPU.SetDataRegister(0, 0);
            model.CPU.SetDataRegister(1, 1);
            model.CPU.SetPC(10);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(14, model.CPU.GetPC());
            Assert.Equal(0x0000ffff, model.CPU.GetDataRegister(0));
            Assert.Equal(1, model.CPU.GetDataRegister(1));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}