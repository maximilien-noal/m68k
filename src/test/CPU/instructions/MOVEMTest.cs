using Miggy;

using Xunit;

namespace M68k.CPU.Instructions.Move
{
    public class MOVEMTest : BasicSetup
    {
        public MOVEMTest() : base()
        {
        }

        [Fact]
        public virtual void TestM2RLong()
        {
            SetInstructionParamW(0x4cd1, 0x0907);
            int base_renamed = codebase + 100;
            model.CPU.SetAddrRegister(1, base_renamed);
            model.MEM.Poke(base_renamed, 305419896, Size.SizeLong);
            model.MEM.Poke(base_renamed + 4, -1431655766, Size.SizeLong);
            model.MEM.Poke(base_renamed + 8, -1061109568, Size.SizeLong);
            model.MEM.Poke(base_renamed + 12, -2023406815, Size.SizeLong);
            model.MEM.Poke(base_renamed + 16, 0x55555555, Size.SizeLong);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(305419896, model.CPU.GetDataRegister(0));
            Assert.Equal(-1431655766, model.CPU.GetDataRegister(1));
            Assert.Equal(-1061109568, model.CPU.GetDataRegister(2));
            Assert.Equal(-2023406815, model.CPU.GetAddrRegister(0));
            Assert.Equal(0x55555555, model.CPU.GetAddrRegister(3));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestM2RWord()
        {
            SetInstructionParamW(0x4c9f, 0x0907);
            model.CPU.Push(0x5555, Size.Word);
            model.CPU.Push(0x4321, Size.Word);
            model.CPU.Push(0xc0c0, Size.Word);
            model.CPU.Push(0xaaaa, Size.Word);
            model.CPU.Push(0x5678, Size.Word);
            model.CPU.SetDataRegister(0, 0x12340c0c);
            model.CPU.SetDataRegister(1, -1431696372);
            model.CPU.SetDataRegister(2, -1061155828);
            model.CPU.SetAddrRegister(0, -2023420916);
            model.CPU.SetAddrRegister(3, 0x55550c0c);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            Assert.Equal(0x00005678, model.CPU.GetDataRegister(0));
            Assert.Equal(-21846, model.CPU.GetDataRegister(1));
            Assert.Equal(-16192, model.CPU.GetDataRegister(2));
            Assert.Equal(0x00004321, model.CPU.GetAddrRegister(0));
            Assert.Equal(0x00005555, model.CPU.GetAddrRegister(3));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestR2MLong()
        {
            SetInstructionParamW(0x48d1, 0x0907);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, -1431655766);
            model.CPU.SetDataRegister(2, -1061109568);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetAddrRegister(3, 0x55555555);
            model.CPU.SetAddrRegister(1, codebase + 100);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            int base_renamed = model.CPU.GetAddrRegister(1);
            Assert.Equal(codebase + 100, base_renamed);
            Assert.Equal(305419896, model.MEM.Peek(base_renamed, Size.SizeLong));
            Assert.Equal(-1431655766, model.MEM.Peek(base_renamed + 4, Size.SizeLong));
            Assert.Equal(-1061109568, model.MEM.Peek(base_renamed + 8, Size.SizeLong));
            Assert.Equal(-2023406815, model.MEM.Peek(base_renamed + 12, Size.SizeLong));
            Assert.Equal(0x55555555, model.MEM.Peek(base_renamed + 16, Size.SizeLong));
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }

        [Fact]
        public virtual void TestR2MWord()
        {
            SetInstructionParamW(0x48a7, 0xe090);
            model.CPU.SetDataRegister(0, 305419896);
            model.CPU.SetDataRegister(1, -1431655766);
            model.CPU.SetDataRegister(2, -1061109568);
            model.CPU.SetAddrRegister(0, -2023406815);
            model.CPU.SetAddrRegister(3, 0x55555555);
            model.CPU.SetCCR((byte)0);
            _ = model.CPU.Execute();
            int stack = model.CPU.GetAddrRegister(7);
            Assert.Equal((short)0x5678, (short)model.MEM.Peek(stack, Size.Word));
            unchecked
            {
                Assert.Equal((short)0xaaaa, (short)model.MEM.Peek(stack + 2, Size.Word));
                Assert.Equal((short)0xc0c0, (short)model.MEM.Peek(stack + 4, Size.Word));
                Assert.Equal((short)0x4321, (short)model.MEM.Peek(stack + 6, Size.Word));
                Assert.Equal((short)0x5555, (short)model.MEM.Peek(stack + 8, Size.Word));
            }
            Assert.False(model.CPU.IsSet(CpuCore.X_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.N_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.Z_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.V_FLAG));
            Assert.False(model.CPU.IsSet(CpuCore.C_FLAG));
        }
    }
}