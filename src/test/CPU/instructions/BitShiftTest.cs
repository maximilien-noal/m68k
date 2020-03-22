//namespace m68k.cpu.instructions
//{
//    using M68k.CPU;
//    using Xunit;

// public class BitShiftTest { private int destReg = 0;

// private int shiftOrRotateValue = 0;

// private int srcReg = 1;

// public BitShiftTest() { SystemModel.MEM = TestMem.create(2048); SystemModel.CPU = new TestCpu(SystemModel.MEM);

// //test vectors just containing the vector number for (int v = 0; v < 256; v++) { int addr = v << 2;

// //test: changed to poke SystemModel.MEM.poke(addr, v, Size.Long); }

// SystemModel.CPU.setAddrRegisterWord(7, 2048); //set up the stack SystemModel.CPU.setUSP(0x0800);
// SystemModel.CPU.setSSP(0x0780); codebase = 0x0400; SystemModel.CPU.setPC(codebase); }

// public BitShiftTest(string test) { }

// ///
// <summary>
// /// Shift count of zero: ///
// <para>X - unaffected V - cleared C - cleared</para>
// ///
// </summary>
// public void TestAsl() { int opcode = 0xE320; //asl.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// public void TestAsr() { int opcode = 0xE220; //asr.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// ///
// <summary>
// /// Shift count of zero: ///
// <para>X - unaffected V - cleared C - cleared</para>
// ///
// </summary>
// public void TestLsl() { int opcode = 0xE328; //lsl.b d1,d0 int d0 = 0x07654321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// public void testLsr() { int opcode = 0xE228; //lsr.b d1,d0 int d0 = 0x07654321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// ///
// <summary>
// /// Shift count of zero: ///
// <para>X - unaffected V - cleared C - cleared</para>
// ///
// </summary>
// public void TestRol() { int opcode = 0xE338; //rol.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// public void TestRor() { int opcode = 0xE238; //ror.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0); }

// ///
// <summary>
// /// Shift count of zero: ///
// <para>X - unaffected V - cleared C - set to the value of the extend bit</para>
// ///
// </summary>
// public void TestRoxl() { int opcode = 0xE330; //roxl.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0, true); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0, true); }

// public void TestRoxr() { int opcode = 0xE230; //roxr.b d1,d0 int d0 = 0x4321;
// TestInstInternal(opcode, CpuFlag.C | CpuFlag.X | CpuFlag.V, d0, true); TestInstInternal(opcode,
// CpuFlag.C | CpuFlag.V, d0, true); }

// private static void AssertFlagStates(int beforeState, bool isRox) { switch (beforeState) { case
// CpuFlag.C | CpuFlag.X | CpuFlag.V: Assert.True("Check X", CPU.isSet(CpuFlag.X)); break;

// case CpuFlag.C | CpuFlag.V: Assert.False("Check X", CPU.isSet(CpuFlag.X)); break;

// default: Assert.False(true); //"Unknown flag combination: " + beforeState break; }
// Assert.False("Check V", CPU.isSet(CpuFlag.V)); if (isRox) { Assert.Equal("Check C",
// CPU.isSet(CpuFlag.X), CPU.isSet(CpuFlag.C)); } else { Assert.False("Check C",
// CPU.isSet(CpuFlag.C)); } Assert.False("Check Z", CPU.isSet(CpuFlag.Z)); Assert.False("Check N",
// CPU.isSet(CpuFlag.N)); }

// private void TestInstInternal(int opcode, int flagState, int d0) { TestInstInternal(opcode,
// flagState, d0, false); }

// private void TestInstInternal(int opcode, int flagState, int d0, bool isRox) {
// TestOpcodeInternal(opcode, flagState, d0, isRox); //byte TestOpcodeInternal(opcode + 0x40,
// flagState, d0, isRox); //word TestOpcodeInternal(opcode + 0x80, flagState, d0, isRox); //long }

// private void TestOpcodeInternal(int opcode, int flagState, int d0, bool isRox) { IInstruction =
// opcode; CPU.setDataRegister(destReg, d0); CPU.setDataRegister(srcReg, shiftOrRotateValue);
// CPU.CCR = 0; CPU.Flags = flagState;

//            CPU.execute();
//            Assert.Equal("Check result", d0, CPU.getDataRegister(destReg));
//            AssertFlagStates(flagState, isRox);
//        }
//    }
//}