namespace Miggy
{
    using M68k.CPU;
    using Miggy.Memory;

    public class BasicSetup
    {
        protected int codebase;

        public BasicSetup()
        {
            SystemModel.MEM = TestMem.Create(2048);
            SystemModel.CPU = new TestCpu(SystemModel.MEM);
            for (int v = 0; v < 256; v++)
            {
                int addr = v << 2;
                SystemModel.MEM.Poke(addr, v, Size.SizeLong);
            }

            SystemModel.CPU.SetAddrRegisterWord(7, 2048);
            SystemModel.CPU.SetUSP(0x0800);
            SystemModel.CPU.SetSSP(0x0780);
            codebase = 0x0400;
            SystemModel.CPU.SetPC(codebase);
        }

        protected virtual void SetInstruction(int opcode)
        {
            SystemModel.MEM.Poke(codebase, opcode, Size.Word);
        }

        protected virtual void SetInstructionParamL(int opcode, int param)
        {
            SystemModel.MEM.Poke(codebase, opcode, Size.Word);
            SystemModel.MEM.Poke(codebase + 2, param, Size.SizeLong);
        }

        protected virtual void SetInstructionParamW(int opcode, int param)
        {
            SystemModel.MEM.Poke(codebase, opcode, Size.Word);
            SystemModel.MEM.Poke(codebase + 2, param, Size.Word);
        }

        protected virtual void TearDown()
        {
            SystemModel.CPU = null;
            SystemModel.MEM = null;
        }
    }
}