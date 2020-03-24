namespace Miggy
{
    using M68k.CPU;

    using Miggy.Memory;

    public class BasicSetup
    {
        protected int codebase;

        protected SystemModel model;

        public BasicSetup()
        {
            Setup();
        }

        protected virtual void SetInstruction(int opcode)
        {
            model.MEM.Poke(codebase, opcode, Size.Word);
        }

        protected virtual void SetInstructionParamL(int opcode, int param)
        {
            model.MEM.Poke(codebase, opcode, Size.Word);
            model.MEM.Poke(codebase + 2, param, Size.SizeLong);
        }

        protected virtual void SetInstructionParamW(int opcode, int param)
        {
            model.MEM.Poke(codebase, opcode, Size.Word);
            model.MEM.Poke(codebase + 2, param, Size.Word);
        }

        protected virtual void Setup()
        {
            model = new SystemModel();
            model.MEM = TestMem.Create(2048);
            model.CPU = new TestCpu(model.MEM);
            for (int v = 0; v < 256; v++)
            {
                int addr = v << 2;
                model.MEM.Poke(addr, v, Size.SizeLong);
            }

            model.CPU.SetAddrRegisterWord(7, 2048);
            model.CPU.SetUSP(0x0800);
            model.CPU.SetSSP(0x0780);
            codebase = 0x0400;
            model.CPU.SetPC(codebase);
        }
    }
}