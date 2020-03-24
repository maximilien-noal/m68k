namespace M68k.Cpu.Instructions
{
    using System;

    using M68k.CPU;

    using Miggy;

    using Xunit;

    public class BitShiftTest : BasicSetup
    {
        private readonly int destReg = 0;

        private readonly int shiftOrRotateValue = 0;

        private readonly int srcReg = 1;

        public BitShiftTest() : base()
        {
        }

        [Fact]
        public virtual void TestAsl()
        {
            int opcode = 0xE320;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestAsr()
        {
            int opcode = 0xE220;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestLsl()
        {
            int opcode = 0xE328;
            int d0 = 0x07654321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestLsr()
        {
            int opcode = 0xE228;
            int d0 = 0x07654321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRol()
        {
            int opcode = 0xE338;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRor()
        {
            int opcode = 0xE238;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRoxl()
        {
            int opcode = 0xE330;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0, true);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0, true);
        }

        [Fact]
        public virtual void TestRoxr()
        {
            int opcode = 0xE230;
            int d0 = 0x4321;
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.XFlag | model.CPU.VFlag, d0, true);
            TestInstInternal(opcode, model.CPU.CFlag | model.CPU.VFlag, d0, true);
        }

        private void AssertFlagStates(int beforeState, bool isRox)
        {
            switch (beforeState)
            {
                case CpuCore.C_FLAG | CpuCore.X_FLAG | CpuCore.V_FLAG:
                    Assert.True(model.CPU.IsSet(model.CPU.XFlag));
                    break;

                case CpuCore.C_FLAG | CpuCore.V_FLAG:
                    Assert.False(model.CPU.IsSet(model.CPU.XFlag));
                    break;

                default:
                    throw new InvalidOperationException($"Unknown flag combination: {beforeState}");
            }

            Assert.False(model.CPU.IsSet(model.CPU.VFlag));
            if (isRox)
            {
                Assert.Equal(model.CPU.IsSet(model.CPU.XFlag), model.CPU.IsSet(model.CPU.CFlag));
            }
            else
            {
                Assert.False(model.CPU.IsSet(model.CPU.CFlag));
            }

            Assert.False(model.CPU.IsSet(model.CPU.ZFlag));
            Assert.False(model.CPU.IsSet(model.CPU.NFlag));
        }

        private void TestInstInternal(int opcode, int flagState, int d0)
        {
            TestInstInternal(opcode, flagState, d0, false);
        }

        private void TestInstInternal(int opcode, int flagState, int d0, bool isRox)
        {
            TestOpcodeInternal(opcode, flagState, d0, isRox);
            TestOpcodeInternal(opcode + 0x40, flagState, d0, isRox);
            TestOpcodeInternal(opcode + 0x80, flagState, d0, isRox);
        }

        private void TestOpcodeInternal(int opcode, int flagState, int d0, bool isRox)
        {
            Setup();
            SetInstruction(opcode);
            model.CPU.SetDataRegister(destReg, d0);
            model.CPU.SetDataRegister(srcReg, shiftOrRotateValue);
            model.CPU.SetCCR(0);
            model.CPU.SetFlags(flagState);
            model.CPU.Execute();
            Assert.Equal(d0, model.CPU.GetDataRegister(destReg));
            AssertFlagStates(flagState, isRox);
        }
    }
}