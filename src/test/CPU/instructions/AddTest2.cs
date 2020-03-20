using M68k.CPU;
using M68k.Memory;
using Xunit;

namespace m68k.cpu.instructions
{
	/// <summary>
	/// ${FILE}
	/// <para>
	/// Federico Berti
	/// </para>
	/// <para>
	/// Copyright 2018
	/// </para>
	/// </summary>

	public class AddTest2
	{
		internal IAddressSpace bus;
		internal MC68000 cpu;


		public AddTest2()
		{
			bus = new MemorySpace(1); //create 1kb of memory for the cpu

			cpu = new MC68000();
			cpu.AddressSpace = bus;
			cpu.Reset();
			cpu.SetAddrRegisterLong(7, 0x200);
		}

		[Fact]
		public virtual void TestADD_byte_zeroFlag()
		{
			bus.WriteWord(4, 0xd402); // add.b d2,d2
									  //this should be true as 0xFFFFFF00_byte -> 00
			TestADDByteZeroFlag(cpu, true, 0xFFFF_FF80, 0xFFFF_FF00);
		}

		[Fact]
		public virtual void TestADDQ_byte_zeroFlag()
		{
			bus.WriteWord(4, 0x5402); // addq.b #2,d2 or addi.b #2, d2
			TestADDByteZeroFlag(cpu, true, 0xFFFF_FFFE, 0xFFFF_FF00);
		}
		[Fact]

		public virtual void TestADD_word_zeroFlag()
		{
			bus.WriteWord(4, 0xd442); // add.w d2,d2
									  //this should be true as 0xFFFFF0000_word -> 0000
			TestADDWordZeroFlag(cpu, true, 0xFFFF_8000, 0xFFFF_0000);
		}
		[Fact]

		public virtual void TestADDQ_word_zeroFlag()
		{
			bus.WriteWord(4, 0x5442); // addq.w #2,d2 or addi.w #2, d2
			TestADDByteZeroFlag(cpu, true, 0x0001_FFFE, 0x0001_0000);
		}

		private void TestADDByteZeroFlag(MC68000 cpu, bool expectedZFlag, long d2_pre, long d2_post)
		{
			cpu.SetPC(4);
			cpu.SetDataRegisterLong(2, (int) d2_pre);
			cpu.Execute();

			Assert.Equal(d2_post, cpu.GetDataRegisterLong(2));
			Assert.Equal(0x00, cpu.GetDataRegisterByte(2));
			Assert.Equal(expectedZFlag, cpu.IsFlagSet(cpu.ZFlag));
		}

		private void TestADDWordZeroFlag(MC68000 cpu, bool expectedZFlag, long d2_pre, long d2_post)
		{
			cpu.SetPC(4);
			cpu.SetDataRegisterLong(2, (int) d2_pre);
			cpu.Execute();

			Assert.Equal(d2_post, cpu.GetDataRegisterLong(2));
			Assert.Equal(0x0000, cpu.GetDataRegisterWord(2));
			Assert.Equal(expectedZFlag, cpu.IsFlagSet(cpu.ZFlag));
		}
	}

}