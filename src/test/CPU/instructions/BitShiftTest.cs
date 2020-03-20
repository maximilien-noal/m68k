using M68k.CPU;
using Xunit;

namespace m68k.cpu.instructions
{
	//public class BitShiftTest
	//{

	//	private int destReg = 0;
	//	private int srcReg = 1;
	//	private int shiftOrRotateValue = 0;

	//	public BitShiftTest(string test)
	//	{
	//	}

	//	/// <summary>
	//	/// Shift count of zero:
	//	/// <para>
	//	/// X - unaffected
	//	/// V - cleared
	//	/// C - cleared
	//	/// </para>
	//	/// </summary>
	//	public virtual void testLsl()
	//	{
	//		int opcode = 0xE328; //lsl.b d1,d0
	//		int d0 = 0x07654321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	public virtual void testLsr()
	//	{
	//		int opcode = 0xE228; //lsr.b d1,d0
	//		int d0 = 0x07654321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	/// <summary>
	//	/// Shift count of zero:
	//	/// <para>
	//	/// X - unaffected
	//	/// V - cleared
	//	/// C - cleared
	//	/// </para>
	//	/// </summary>
	//	public virtual void testAsl()
	//	{
	//		int opcode = 0xE320; //asl.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	public virtual void testAsr()
	//	{
	//		int opcode = 0xE220; //asr.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	/// <summary>
	//	/// Shift count of zero:
	//	/// <para>
	//	/// X - unaffected
	//	/// V - cleared
	//	/// C - cleared
	//	/// </para>
	//	/// </summary>
	//	public virtual void testRol()
	//	{
	//		int opcode = 0xE338; //rol.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	public virtual void testRor()
	//	{
	//		int opcode = 0xE238; //ror.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0);
	//	}

	//	/// <summary>
	//	/// Shift count of zero:
	//	/// <para>
	//	/// X - unaffected
	//	/// V - cleared
	//	/// C - set to the value of the extend bit
	//	/// </para>
	//	/// </summary>
	//	public virtual void testRoxl()
	//	{
	//		int opcode = 0xE330; //roxl.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0, true);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0, true);
	//	}

	//	public virtual void testRoxr()
	//	{
	//		int opcode = 0xE230; //roxr.b d1,d0
	//		int d0 = 0x4321;
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0, true);
	//		testInstInternal(opcode, CpuFlag.C | CpuFlag.V, d0, true);
	//	}

	//	private void testInstInternal(int opcode, int flagState, int d0)
	//	{
	//		testInstInternal(opcode, flagState, d0, false);
	//	}

	//	private void testInstInternal(int opcode, int flagState, int d0, bool isRox)
	//	{
	//		testOpcodeInternal(opcode, flagState, d0, isRox); //byte
	//		testOpcodeInternal(opcode + 0x40, flagState, d0, isRox); //word
	//		testOpcodeInternal(opcode + 0x80, flagState, d0, isRox); //long
	//	}

	//	private void testOpcodeInternal(int opcode, int flagState, int d0, bool isRox)
	//	{
	//		SetUp();
	//		IInstruction = opcode;
	//		CPU.setDataRegister(destReg, d0);
	//		CPU.setDataRegister(srcReg, shiftOrRotateValue);
	//		CPU.CCR = 0;
	//		CPU.Flags = flagState;

	//		CPU.execute();
	//		Assert.Equal("Check result", d0, CPU.getDataRegister(destReg));
	//		assertFlagStates(flagState, isRox);
	//	}

	//	private static void assertFlagStates(int beforeState, bool isRox)
	//	{
	//		switch (beforeState)
	//		{
	//			case CpuFlag.C | CpuFlag.X | CpuFlag.V:
	//				Assert.True("Check X", CPU.isSet(CpuFlag.X));
	//				break;
	//			case CpuFlag.C | CpuFlag.V:
	//				Assert.False("Check X", CPU.isSet(CpuFlag.X));
	//				break;
	//			default:
	//				Assert.fail("Unknown flag combination: " + beforeState);
	//				break;
	//		}
	//		Assert.False("Check V", CPU.isSet(CpuFlag.V));
	//		if (isRox)
	//		{
	//			Assert.Equal("Check C", CPU.isSet(CpuFlag.X), CPU.isSet(CpuFlag.C));
	//		}
	//		else
	//		{
	//			Assert.False("Check C", CPU.isSet(CpuFlag.C));
	//		}
	//		Assert.False("Check Z", CPU.isSet(CpuFlag.Z));
	//		Assert.False("Check N", CPU.isSet(CpuFlag.N));
	//	}
	//}
}