using M68k.CPU.Instructions;

using System;
using System.Globalization;
using System.Text;

namespace M68k.CPU
{
    public class MC68000 : CpuCore, IInstructionSet
    {
        private readonly IInstruction[] instructionsTable;

        private readonly IInstruction unknown;

        private uint loadedOperations;

        static MC68000()
        {
            InitProperties();
        }

        public MC68000()
        {
            SrcEAHandler = null;
            DstEAHandler = null;
            Memory = null;
            DisasmBuffer = new StringBuilder(64);
            InitEAHandlers();
            instructionsTable = new IInstruction[65536];
            for (uint i = 0; i < 65536; i++)
                instructionsTable[i] = null;
            unknown = new UNKNOWN(this);
            loadedOperations = 0;
            LoadInstructionSet();
        }

        public virtual void AddInstruction(uint opcode, IInstruction instruction)
        {
            if (instruction is null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }

            IInstruction current = instructionsTable[opcode];
            if (current == null)
            {
                instructionsTable[opcode] = instruction;
                loadedOperations++;
            }
            else
            {
                throw new ArgumentException($"Attempted to overwrite existing instruction [{current.GetType().Name}] at 0x{opcode.ToString("x", CultureInfo.InvariantCulture)} with [{instruction.GetType().Name}]");
            }
        }

        public override uint Execute()
        {
            CurrentInstructionAddress = RegPc;
            uint opcode = FetchPCWord();
            IInstruction i = instructionsTable[opcode];
            if (i != null)
            {
                return i.Execute(opcode);
            }
            else
            {
                RegPc = CurrentInstructionAddress;
                return unknown.Execute(opcode);
            }
        }

        public override IInstruction GetInstructionAt(uint address)
        {
            uint opcode = ReadMemoryWord(address);
            return GetInstructionFor(opcode);
        }

        public override IInstruction GetInstructionFor(uint opcode)
        {
            IInstruction i = instructionsTable[opcode];
            if (i == null)
                i = unknown;
            return i;
        }

        public override string ToString()
        {
            return loadedOperations.ToString(CultureInfo.CurrentCulture);
        }

        private static void InitProperties()
        {
            if (System.Configuration.ConfigurationManager.AppSettings == null)
            {
                return;
            }
            var config = System.Configuration.ConfigurationManager.AppSettings.Get("68k.broken.tas");
            if (config != null && bool.TryParse(config, out var mustEmulateBrokenTAS) && mustEmulateBrokenTAS)
            {
                TAS.EmulateBrokenTAS = true;
                Console.WriteLine("Emulating broken TAS instruction");
            }
        }

        private void LoadInstructionSet()
        {
            new ABCD(this).Register(this);
            new ADD(this).Register(this);
            new ADDA(this).Register(this);
            new ADDI(this).Register(this);
            new ADDQ(this).Register(this);
            new ADDX(this).Register(this);
            new ANDInstruction(this).Register(this);
            new ANDI(this).Register(this);
            new AndiToSr(this).Register(this);
            new AndiToCcr(this).Register(this);
            new ASL(this).Register(this);
            new ASR(this).Register(this);
            new BCC(this).Register(this);
            new BCHG(this).Register(this);
            new BCLR(this).Register(this);
            new BSET(this).Register(this);
            new BTST(this).Register(this);
            new CHK(this).Register(this);
            new CLR(this).Register(this);
            new CMP(this).Register(this);
            new CMPA(this).Register(this);
            new CMPI(this).Register(this);
            new CMPM(this).Register(this);
            new DBCC(this).Register(this);
            new DIVS(this).Register(this);
            new DIVU(this).Register(this);
            new EOR(this).Register(this);
            new EORI(this).Register(this);
            new EoriToCcr(this).Register(this);
            new EoriToSr(this).Register(this);
            new EXG(this).Register(this);
            new EXT(this).Register(this);
            new ILLEGAL(this).Register(this);
            new JMP(this).Register(this);
            new JSR(this).Register(this);
            new LEA(this).Register(this);
            new LINK(this).Register(this);
            new LSL(this).Register(this);
            new LSR(this).Register(this);
            new MOVE(this).Register(this);
            new MoveToCcr(this).Register(this);
            new MoveToSr(this).Register(this);
            new MoveFromSr(this).Register(this);
            new MoveToUsp(this).Register(this);
            new MOVEA(this).Register(this);
            new MOVEM(this).Register(this);
            new MOVEP(this).Register(this);
            new MOVEQ(this).Register(this);
            new MULS(this).Register(this);
            new MULU(this).Register(this);
            new NBCD(this).Register(this);
            new NEG(this).Register(this);
            new NEGX(this).Register(this);
            new NOP().Register(this);
            new NOTInstruction(this).Register(this);
            new ORInstruction(this).Register(this);
            new ORI(this).Register(this);
            new OriToSr(this).Register(this);
            new OriToCcr(this).Register(this);
            new PEA(this).Register(this);
            new RESET(this).Register(this);
            new ROL(this).Register(this);
            new ROR(this).Register(this);
            new ROXL(this).Register(this);
            new ROXR(this).Register(this);
            new RTE(this).Register(this);
            new RTR(this).Register(this);
            new RTS(this).Register(this);
            new SBCD(this).Register(this);
            new Scc(this).Register(this);
            new STOPInstruction(this).Register(this);
            new SUBInstruction(this).Register(this);
            new SUBA(this).Register(this);
            new SUBI(this).Register(this);
            new SUBQ(this).Register(this);
            new SUBX(this).Register(this);
            new SWAP(this).Register(this);
            new TAS(this).Register(this);
            new TRAP(this).Register(this);
            new TRAPV(this).Register(this);
            new TST(this).Register(this);
            new UNLK(this).Register(this);
        }
    }
}