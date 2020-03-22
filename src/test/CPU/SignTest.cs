namespace M68k.CPU
{
    using Miggy;

    using Xunit;

    public class SignTest : BasicSetup
    {
        public SignTest() : base()
        {
        }

        [Fact]
        public virtual void TestSignExtendByte()
        {
            SystemModel.MEM.Poke(codebase, 0x80, Size.Byte);
            int val = SystemModel.MEM.Peek(codebase, Size.Byte);
            Assert.True(val >= 0);
            val = (byte)SystemModel.MEM.Peek(codebase, Size.Byte);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendFetchByte()
        {
            SystemModel.MEM.Poke(codebase, 0x0080, Size.Word);
            int val = SystemModel.CPU.Fetch(Size.Byte);
            Assert.True(val >= 0);
            Assert.Equal(0x0080, val);
            SystemModel.CPU.SetPC(codebase);
            val = (byte)SystemModel.CPU.Fetch(Size.Byte);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendFetchWord()
        {
            SystemModel.MEM.Poke(codebase, 0x8000, Size.Word);
            int val = SystemModel.CPU.Fetch(Size.Word);
            Assert.True(val >= 0);
            Assert.Equal(0x8000, val);
            SystemModel.CPU.SetPC(codebase);
            val = (short)SystemModel.CPU.Fetch(Size.Word);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendWord()
        {
            SystemModel.MEM.Poke(codebase, 0x8000, Size.Word);
            int val = SystemModel.MEM.Peek(codebase, Size.Word);
            Assert.True(val >= 0);
            val = (short)SystemModel.MEM.Peek(codebase, Size.Word);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSizeSignExtend()
        {
            int val = 0x0080;
            int result = SystemModel.CPU.SignExtendByte(val);
            Assert.True(result < 0);
            val = 0x8000;
            result = SystemModel.CPU.SignExtendWord(val);
            Assert.True(result < 0);
        }
    }
}