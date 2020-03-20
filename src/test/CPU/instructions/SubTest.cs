using M68k.CPU;
using M68k.Memory;
using Xunit;

namespace m68k.cpu.instructions
{
	public class SubTest
	{
		internal IAddressSpace bus;
		internal MC68000 cpu;

		public SubTest()
		{
			bus = new MemorySpace(1); //create 1kb of memory for the cpu
			cpu = new MC68000();
			cpu.AddressSpace = bus;
			cpu.Reset();
			cpu.SetAddrRegisterLong(7, 0x200);
		}

		public virtual void TestSUB_byte_zeroFlag()
		{
			bus.WriteWord(4, 0x9402); // sub.b d2,d2
			TestSUB_byte_zeroFlag(cpu, true, 0xFFFF_FF80, 0xFFFF_FF00);
		}

		public virtual void TestSUBQ_byte_zeroFlag()
		{
			bus.WriteWord(4, 0x5502); // subi.b #2,d2
			TestSUB_byte_zeroFlag(cpu, true, 0x0001_0102, 0x0001_0100);
		}

		public virtual void TestSUBI_byte_zeroFlag()
		{
			bus.WriteWord(4, 0x0402); // subi.b #2,d2
			bus.WriteWord(6, 2);
			TestSUB_byte_zeroFlag(cpu, true, 0x0001_0102, 0x0001_0100);
		}

		private void TestSUB_byte_zeroFlag(MC68000 cpu, bool expectedZFlag, long d2_pre, long d2_post)
		{
			cpu.SetPC(4);
			cpu.SetDataRegisterLong(2, (int) d2_pre);
			cpu.Execute();
			Assert.Equal(d2_post, cpu.GetDataRegisterLong(2));
			Assert.Equal(0x00, cpu.GetDataRegisterByte(2));
			Assert.Equal(expectedZFlag, cpu.IsFlagSet(cpu.ZFlag));
		}
	}

}