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
            model.MEM.Poke(codebase, 0x80, Size.Byte);
            int val = model.MEM.Peek(codebase, Size.Byte);
            Assert.True(val >= 0);
            var valNoCast = model.MEM.Peek(codebase, Size.Byte);
            val = (sbyte)valNoCast;
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendFetchByte()
        {
            model.MEM.Poke(codebase, 0x0080, Size.Word);
            int val = model.CPU.Fetch(Size.Byte);
            Assert.True(val >= 0);
            Assert.Equal(0x0080, val);
            model.CPU.SetPC(codebase);
            val = (sbyte)model.CPU.Fetch(Size.Byte);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendFetchWord()
        {
            model.MEM.Poke(codebase, 0x8000, Size.Word);
            int val = model.CPU.Fetch(Size.Word);
            Assert.True(val >= 0);
            Assert.Equal(0x8000, val);
            model.CPU.SetPC(codebase);
            val = (short)model.CPU.Fetch(Size.Word);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSignExtendWord()
        {
            model.MEM.Poke(codebase, 0x8000, Size.Word);
            int val = model.MEM.Peek(codebase, Size.Word);
            Assert.True(val >= 0);
            val = (short)model.MEM.Peek(codebase, Size.Word);
            Assert.True(val < 0);
        }

        [Fact]
        public virtual void TestSizeSignExtend()
        {
            int val = 0x0080;
            int result = model.CPU.SignExtendByte(val);
            Assert.True(result < 0);
            val = 0x8000;
            result = model.CPU.SignExtendWord(val);
            Assert.True(result < 0);
        }
    }
}