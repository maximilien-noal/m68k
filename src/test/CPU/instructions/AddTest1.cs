

namespace m68k.cpu.instructions
{
	using M68k.CPU;
	using M68k.Memory;
	using Xunit;

	public class AddTest1
	{
		internal IAddressSpace bus;
		internal MC68000 cpu;

		public AddTest1()
		{
			bus = new MemorySpace(1); //create 1kb of memory for the cpu
			cpu = new MC68000();
			cpu.AddressSpace = bus;
			cpu.Reset();
			cpu.SetAddrRegisterLong(7, 0x200);
		}

		[Fact]
		public virtual void TestADD()
		{
			cpu.SetPC(4);
			cpu.SetDataRegisterByte(0, 0x40);
			cpu.SetDataRegisterByte(1, 0x80);
			bus.WriteWord(4, 0xd001); // add.b d1,d0
			int ticks = cpu.Execute();
			Assert.Equal(6, cpu.GetPC());
			Assert.Equal(0xc0, cpu.GetDataRegisterByte(0));
			Assert.Equal(0x80, cpu.GetDataRegisterByte(1));
			Assert.Equal(4, ticks);
			Assert.False(cpu.IsFlagSet(cpu.CFlag));
			Assert.False(cpu.IsFlagSet(cpu.VFlag));
			Assert.False(cpu.IsFlagSet(cpu.ZFlag));
			Assert.True(cpu.IsFlagSet(cpu.NFlag));
			Assert.False(cpu.IsFlagSet(cpu.XFlag));

			cpu.SetPC(4);
			cpu.SetDataRegisterWord(0, 0x8000);
			cpu.SetDataRegisterWord(1, 0x8500);
			bus.WriteWord(4, 0xd041); // add.w d1,d0
			ticks = cpu.Execute();
			Assert.Equal(6, cpu.GetPC());
			Assert.Equal(0x0500, cpu.GetDataRegisterWord(0));
			Assert.Equal(0x8500, cpu.GetDataRegisterWord(1));
			Assert.Equal(4, ticks);
			Assert.True(cpu.IsFlagSet(cpu.CFlag));
			Assert.True(cpu.IsFlagSet(cpu.VFlag));
			Assert.False(cpu.IsFlagSet(cpu.ZFlag));
			Assert.False(cpu.IsFlagSet(cpu.NFlag));
			Assert.True(cpu.IsFlagSet(cpu.XFlag));


			cpu.SetPC(4);
			unchecked
			{
				cpu.SetDataRegisterLong(0, (int)0xfffffffc);
			}
			cpu.SetDataRegisterLong(1, 0x04);
			bus.WriteWord(4, 0xd081); // add.l d1,d0
			ticks = cpu.Execute();
			Assert.Equal(6, cpu.GetPC());
			Assert.Equal(0, cpu.GetDataRegisterLong(0));
			Assert.Equal(0x04, cpu.GetDataRegisterWord(1));
			Assert.Equal(6, ticks);
			Assert.True(cpu.IsFlagSet(cpu.CFlag));
			Assert.False(cpu.IsFlagSet(cpu.VFlag));
			Assert.True(cpu.IsFlagSet(cpu.ZFlag));
			Assert.False(cpu.IsFlagSet(cpu.NFlag));
			Assert.True(cpu.IsFlagSet(cpu.XFlag));
		}
	}

}