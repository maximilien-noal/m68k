namespace M68k.Cpu.Instructions
{
    using Miggy;

    using System;

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
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestAsr()
        {
            int opcode = 0xE220;
            int d0 = 0x4321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestLsl()
        {
            int opcode = 0xE328;
            int d0 = 0x07654321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestLsr()
        {
            int opcode = 0xE228;
            int d0 = 0x07654321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRol()
        {
            int opcode = 0xE338;
            int d0 = 0x4321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRor()
        {
            int opcode = 0xE238;
            int d0 = 0x4321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0);
        }

        [Fact]
        public virtual void TestRoxl()
        {
            int opcode = 0xE330;
            int d0 = 0x4321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0, true);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0, true);
        }

        [Fact]
        public virtual void TestRoxr()
        {
            int opcode = 0xE230;
            int d0 = 0x4321;
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.XFlag | SystemModel.CPU.VFlag, d0, true);
            TestInstInternal(opcode, SystemModel.CPU.CFlag | SystemModel.CPU.VFlag, d0, true);
        }

        private static void AssertFlagStates(int beforeState, bool isRox)
        {
            if (beforeState == SystemModel.CPU.CFlag | beforeState == SystemModel.CPU.XFlag | beforeState == SystemModel.CPU.VFlag)
            {
                Assert.True(SystemModel.CPU.IsSet(SystemModel.CPU.XFlag));
            }
            else if (beforeState == SystemModel.CPU.CFlag | beforeState == SystemModel.CPU.VFlag)
            {
                Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.XFlag));
            }
            else
            {
                throw new InvalidOperationException($"Unknown flag combination: {beforeState}");
            }

            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.VFlag));
            if (isRox)
            {
                Assert.Equal(SystemModel.CPU.IsSet(SystemModel.CPU.XFlag), SystemModel.CPU.IsSet(SystemModel.CPU.CFlag));
            }
            else
            {
                Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.CFlag));
            }

            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.ZFlag));
            Assert.False(SystemModel.CPU.IsSet(SystemModel.CPU.NFlag));
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
            SetInstruction(opcode);
            SystemModel.CPU.SetDataRegister(destReg, d0);
            SystemModel.CPU.SetDataRegister(srcReg, shiftOrRotateValue);
            SystemModel.CPU.SetCCR(0);
            SystemModel.CPU.SetFlags(flagState);
            SystemModel.CPU.Execute();
            Assert.Equal(d0, SystemModel.CPU.GetDataRegister(destReg));
            AssertFlagStates(flagState, isRox);
        }
    }
}