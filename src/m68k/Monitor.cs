namespace M68k
{
    using M68k.CPU;
    using M68k.Memory;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class Monitor
    {
        private readonly List<uint> breakpoints;

        private StringBuilder buffer;

        private readonly ICPU cpu;

        private readonly IAddressSpace memory;

        private bool autoRegs;

        private TextReader reader;

        private bool running;

        private bool showBytes;

        private TextWriter writer;

        public Monitor(ICPU icpu, IAddressSpace imemory)
        {
            cpu = icpu;
            memory = imemory;
            buffer = new StringBuilder(128);
            showBytes = false;
            autoRegs = false;
            breakpoints = new List<uint>();
        }

        public static void Main(string[] args)
        {
            if (args is null || args.All(x => string.IsNullOrWhiteSpace(x)))
            {
                throw new ArgumentNullException(nameof(args));
            }

            uint memSize = 512;
            if (args.Length == 1)
            {
                try
                {
                    memSize = uint.Parse(args[0], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"Invalid number: {args[0]}");
                    Console.WriteLine("Usage: m68k.Monitor [memory size Kb]");
                    Environment.Exit(-1);
                }
            }

            Console.WriteLine("m68k Monitor v0.1 - Copyright 2008-2010 Tony Headford");
            using (var newMemory = new MemorySpace(memSize))
            {
                var newCpu = new MC68000();
                newCpu.SetAddressSpace(newMemory);
                newCpu.Reset();
                Monitor monitor = new Monitor(newCpu, newMemory);
                monitor.Run();
            }
        }

        public void Run()
        {
            writer = System.Console.Out;
            reader = System.Console.In;

            running = true;
            while (running)
            {
                try
                {
                    writer.Write("> ");
                    writer.Flush();
                    HandleCommand(reader.ReadLine());
                    if (autoRegs)
                    {
                        DumpInfo();
                    }
                }
                catch (IOException e)
                {
                    Console.Write(e.StackTrace);
                }
            }
        }

        protected void DumpInfo()
        {
            writer.WriteLine();
            writer.WriteLine("D0: {0}   D4: {1}   A0: {2}   A4: {3}     PC: {4}",
                cpu.GetDataRegisterLong(0).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetDataRegisterLong(4).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(0).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(4).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetPC().ToString("x8", CultureInfo.InvariantCulture));
            writer.WriteLine("D1: {0}   D5: {1}   A1: {2}   A5: {3}     SR:  {4} {5}",
                cpu.GetDataRegisterLong(1).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetDataRegisterLong(5).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(1).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(5).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetSR().ToString("x4", CultureInfo.InvariantCulture),
                MakeFlagView());
            writer.WriteLine("D2: {0}   D6: {1}   A2: {2}   A6: {3}     USP: {4}",
                cpu.GetDataRegisterLong(2).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetDataRegisterLong(6).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(2).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(6).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetUSP());
            writer.WriteLine("D3: {0}   D7: {1}   A3: {2}   A7: {3}     SSP: {4}",
                cpu.GetDataRegisterLong(3).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetDataRegisterLong(7).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(3).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetAddrRegisterLong(7).ToString("x8", CultureInfo.InvariantCulture),
                cpu.GetSSP().ToString("x8", CultureInfo.InvariantCulture));
            buffer.Clear();
            uint addr = cpu.GetPC();
            if (addr < 0 || addr >= memory.Size())
            {
                buffer.Append($"{addr.ToString("x8", CultureInfo.InvariantCulture)}   ????");
            }
            else
            {
                uint opcode = cpu.ReadMemoryWord(addr);
                IInstruction i = cpu.GetInstructionFor(opcode);
                DisassembledInstruction di = i.Disassemble(addr, opcode);
                if (showBytes)
                {
                    di.FormatInstruction(buffer);
                }
                else
                {
                    di.ShortFormat(buffer);
                }
            }

            writer.WriteLine($"==> {buffer}{Environment.NewLine}");
        }

        protected char GetPrintable(uint val)
        {
            if (val < ' ' || val > '~')
            {
                return '.';
            }

            return (char)val;
        }

        protected void HandleAddrRegs(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            string reg = tokens[0].Trim();
            if (reg.Length != 2)
            {
                writer.WriteLine($"Bad identifier [{reg}]");
                return;
            }

            var r = reg[1] - '0';
            if (r < 0 || r > 7)
            {
                writer.WriteLine($"Bad identifier [{reg}]");
                return;
            }

            if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetAddrRegisterLong((uint)r, value);
            }
            else
            {
                writer.WriteLine("A{0}: {1}", r, cpu.GetAddrRegisterLong((uint)r).ToString("x8", CultureInfo.InvariantCulture));
            }
        }

        protected void HandleAutoRegs(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length > 1)
            {
                if (tokens[1].Trim().ToUpperInvariant().Equals("ON", StringComparison.InvariantCulture))
                {
                    autoRegs = true;
                }
                else if (tokens[1].Trim().ToUpperInvariant().Equals("OFF", StringComparison.InvariantCulture))
                {
                    autoRegs = false;
                }
            }

            writer.WriteLine("autoregs is " + (autoRegs ? "on" : "off"));
        }

        protected void HandleBreakPoints(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length > 1)
            {
                try
                {
                    uint addr = ParseInt(tokens[1]);
                    if (breakpoints.Contains(addr))
                    {
                        breakpoints.Remove(addr);
                    }
                    else
                    {
                        breakpoints.Add(addr);
                    }
                }
                catch (FormatException)
                {
                    return;
                }
            }

            writer.WriteLine("Breakpoints:");
            foreach (uint bp in breakpoints)
            {
                writer.WriteLine(bp.ToString("x", CultureInfo.InvariantCulture));
            }
        }

        protected void HandleCCR(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length == 1)
            {
                writer.WriteLine($"CCR: {cpu.GetCCRegister().ToString("x", CultureInfo.InvariantCulture)} {MakeFlagView()}");
            }
            else if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetCCRegister(value);
            }
            else
            {
                writer.WriteLine($"usage: {tokens[0]} [value]");
            }
        }

        protected void HandleCommand(string line)
        {
            if (line is null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            string[] tokens = line.Split(' ');
            string cmd = tokens[0].Trim().ToUpperInvariant();
            if (cmd.Length > 0)
            {
                if (cmd.Equals("Q", StringComparison.InvariantCulture))
                {
                    running = false;
                }
                else if (cmd.Equals("R", StringComparison.InvariantCulture))
                {
                    DumpInfo();
                }
                else if (cmd.Equals("PC", StringComparison.InvariantCulture))
                {
                    HandlePC(tokens);
                }
                else if (cmd.Equals("D", StringComparison.InvariantCulture))
                {
                    HandleDisassemble(tokens);
                }
                else if (cmd.Equals("B", StringComparison.InvariantCulture))
                {
                    HandleBreakPoints(tokens);
                }
                else if (cmd.Equals("SR", StringComparison.InvariantCulture))
                {
                    HandleSR(tokens);
                }
                else if (cmd.Equals("CCR", StringComparison.InvariantCulture))
                {
                    HandleCCR(tokens);
                }
                else if (cmd.Equals("USP", StringComparison.InvariantCulture))
                {
                    HandleUSP(tokens);
                }
                else if (cmd.Equals("SSP", StringComparison.InvariantCulture))
                {
                    HandleSSP(tokens);
                }
                else if (cmd.Equals("ML", StringComparison.InvariantCulture))
                {
                    HandleMemLong(tokens);
                }
                else if (cmd.Equals("MW", StringComparison.InvariantCulture))
                {
                    HandleMemWord(tokens);
                }
                else if (cmd.Equals("MB", StringComparison.InvariantCulture))
                {
                    HandleMemByte(tokens);
                }
                else if (cmd.Equals("M", StringComparison.InvariantCulture))
                {
                    HandleMemDump(tokens);
                }
                else if (cmd.Equals("S", StringComparison.InvariantCulture))
                {
                    HandleStep();
                }
                else if (cmd.Equals("G", StringComparison.InvariantCulture))
                {
                    HandleGo();
                }
                else if (cmd.Equals("AUTOREGS", StringComparison.InvariantCulture))
                {
                    HandleAutoRegs(tokens);
                }
                else if (cmd.Equals("SHOWBYTES", StringComparison.InvariantCulture))
                {
                    HandleShowBytes(tokens);
                }
                else if (cmd.Equals("LOAD", StringComparison.InvariantCulture))
                {
                    HandleLoad(tokens);
                }
                else if (cmd.StartsWith("D", StringComparison.InvariantCulture))
                {
                    HandleDataRegs(tokens);
                }
                else if (cmd.StartsWith("A", StringComparison.InvariantCulture))
                {
                    HandleAddrRegs(tokens);
                }
                else if (cmd.Equals("?", StringComparison.InvariantCulture) || cmd.Equals("H", StringComparison.InvariantCulture) || cmd.Equals("HELP", StringComparison.InvariantCulture))
                {
                    ShowHelp();
                }
                else
                {
                    writer.WriteLine($"Unknown command: {tokens[0]}");
                }
            }
        }

        protected void HandleDataRegs(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            string reg = tokens[0].Trim();
            if (reg.Length != 2)
            {
                writer.WriteLine($"Bad identifier [{reg}]");
                return;
            }

            var r = reg[1] - '0';
            if (r < 0 || r > 7)
            {
                writer.WriteLine($"Bad identifier [{reg}]");
                return;
            }

            if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetDataRegisterLong((uint)r, value);
            }
            else
            {
                writer.WriteLine("D{0}: {1}", r.ToString("d", CultureInfo.InvariantCulture), cpu.GetDataRegisterLong((uint)r).ToString("x", CultureInfo.InvariantCulture));
            }
        }

        protected void HandleDisassemble(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            uint start;
            uint num_instructions = 8;
            if (tokens.Length > 2)
            {
                try
                {
                    num_instructions = ParseInt(tokens[2]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Invalid instruction count: {tokens[2]}");
                    return;
                }
            }

            if (tokens.Length > 1)
            {
                string address = tokens[1];
                try
                {
                    start = ParseInt(address);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Unknown address [{address}]");
                    return;
                }
            }
            else
            {
                start = cpu.GetPC();
            }

            uint count = 0;
            buffer = new StringBuilder(80);
            while (start < memory.Size() && count < num_instructions)
            {
                buffer.Clear();
                uint opcode = cpu.ReadMemoryWord(start);
                IInstruction i = cpu.GetInstructionFor(opcode);
                DisassembledInstruction di = i.Disassemble(start, opcode);
                if (showBytes)
                {
                    di.FormatInstruction(buffer);
                }
                else
                {
                    di.ShortFormat(buffer);
                }

                writer.WriteLine(buffer.ToString());
                start += di.Size();
                count++;
            }
        }

        protected void HandleGo()
        {
            uint count = 0;
            bool going = true;
            while (running && going)
            {
                try
                {
                    uint time = cpu.Execute();
                    count += time;
                    uint addr = cpu.GetPC();
                    if (breakpoints.Contains(addr))
                    {
                        writer.WriteLine("BREAKPOINT");
                        going = false;
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    writer.Write(e.StackTrace);
                    going = false;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            writer.WriteLine("[Consumed {0} ticks]", count);
        }

        protected void HandleLoad(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length != 3)
            {
                writer.WriteLine("usage: load <address> <file>");
                return;
            }

            uint address;
            try
            {
                address = ParseInt(tokens[1]);
            }
            catch (FormatException)
            {
                writer.WriteLine($"Invalid address specified [{tokens[1]}]");
                return;
            }

            string filePath = tokens[2];
            if (!File.Exists(filePath))
            {
                writer.WriteLine($"Cannot find file [{tokens[2]}]");
                return;
            }

            if (address + (uint)filePath.Length >= memory.Size())
            {
                writer.WriteLine($"Need larger memory to load this file at {tokens[1]}");
                return;
            }

            try
            {
                byte[] fileContents = File.ReadAllBytes(filePath);
                foreach (var item in fileContents)
                {
                    memory.WriteByte(address, item);
                    address++;
                }
            }
            catch (IOException e)
            {
                writer.Write(e.StackTrace);
            }
        }

        protected void HandleMemByte(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length != 2 && tokens.Length != 3)
            {
                writer.WriteLine("usage: mb <address> [value]");
            }
            else
            {
                string address = tokens[1];
                if (tokens.Length == 2)
                {
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                        }
                        else
                        {
                            writer.WriteLine("{0} {1}", addr.ToString("x", CultureInfo.InvariantCulture), cpu.ReadMemoryByte(addr).ToString("x", CultureInfo.InvariantCulture));
                        }
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
                else
                {
                    string value = tokens[2];
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                            return;
                        }

                        uint v = ParseInt(value);
                        cpu.WriteMemoryByte(addr, v);
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
            }
        }

        protected void HandleMemDump(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length != 2)
            {
                writer.WriteLine("usage: m <start address>");
                return;
            }

            string address = tokens[1];
            uint size = memory.Size();
            try
            {
                uint addr = ParseInt(address);
                if (addr < 0 || addr >= size)
                {
                    writer.WriteLine("Address out of range");
                    return;
                }

                StringBuilder sb = new StringBuilder(80);
                StringBuilder asc = new StringBuilder(16);
                for (uint y = 0; y < 8 && addr < size; y++)
                {
                    sb.Append($"{addr.ToString("x8", CultureInfo.InvariantCulture)}").Append("  ");
                    for (uint x = 0; x < 16 && addr < size; x++)
                    {
                        uint b = cpu.ReadMemoryByte(addr);
                        sb.Append($"{b.ToString("x2", CultureInfo.InvariantCulture)}");
                        asc.Append(GetPrintable(b));
                        addr++;
                    }

                    if (sb.Length < 48)
                    {
                        for (int n = sb.Length; n < 48; n++)
                        {
                            sb.Append(" ");
                        }
                    }

                    sb.Append("    ").Append(asc);
                    writer.WriteLine(sb.ToString());
                    sb.Clear();
                    asc.Clear();
                }
            }
            catch (FormatException)
            {
                writer.WriteLine($"Unknown address [{address}]");
            }
        }

        protected void HandleMemLong(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length != 2 && tokens.Length != 3)
            {
                writer.WriteLine("usage: ml <address> [value]");
            }
            else
            {
                string address = tokens[1];
                if (tokens.Length == 2)
                {
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                            return;
                        }

                        writer.WriteLine("{0} {1}", addr.ToString("x", CultureInfo.InvariantCulture), cpu.ReadMemoryLong(addr).ToString("x", CultureInfo.InvariantCulture));
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
                else
                {
                    string value = tokens[2];
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                            return;
                        }

                        uint v = ParseInt(value);
                        cpu.WriteMemoryLong(addr, v);
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
            }
        }

        protected void HandleMemWord(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length != 2 && tokens.Length != 3)
            {
                writer.WriteLine("usage: mw <address> [value]");
            }
            else
            {
                string address = tokens[1];
                if (tokens.Length == 2)
                {
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                            return;
                        }

                        writer.WriteLine("{0} {1}", addr.ToString("x", CultureInfo.InvariantCulture), cpu.ReadMemoryWord(addr).ToString("x", CultureInfo.InvariantCulture));
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
                else
                {
                    string value = tokens[2];
                    try
                    {
                        uint addr = ParseInt(address);
                        if (addr < 0 || addr >= memory.Size())
                        {
                            writer.WriteLine("Address out of range");
                            return;
                        }

                        uint v = ParseInt(value);
                        cpu.WriteMemoryWord(addr, v);
                    }
                    catch (FormatException e)
                    {
                        writer.Write(e.StackTrace);
                    }
                }
            }
        }

        protected void HandlePC(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length == 1)
            {
                writer.Write($"PC: {cpu.GetPC().ToString("x8", CultureInfo.InvariantCulture)}");
            }
            else if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetPC(value);
            }
            else
            {
                writer.WriteLine($"usage: {tokens[0]} [value]");
            }
        }

        protected void HandleShowBytes(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length > 1)
            {
                if (tokens[1].Trim().ToUpperInvariant().Equals("ON", StringComparison.InvariantCulture))
                {
                    showBytes = true;
                }
                else if (tokens[1].Trim().ToUpperInvariant().Equals("OFF", StringComparison.InvariantCulture))
                {
                    showBytes = false;
                }
            }

            writer.WriteLine($"showbytes is {(showBytes ? "on" : "off")}");
        }

        protected void HandleSR(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length == 1)
            {
                writer.WriteLine("SR: {0}", cpu.GetSR().ToString("x", CultureInfo.InvariantCulture));
            }
            else if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetSR(value);
            }
            else
            {
                writer.WriteLine($"usage: {tokens[0]} [value]");
            }
        }

        protected void HandleSSP(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length == 1)
            {
                writer.WriteLine("SSP: {0}", cpu.GetSSP().ToString("x", CultureInfo.InvariantCulture));
            }
            else if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetSSP(value);
            }
            else
            {
                writer.WriteLine($"usage: {tokens[0]} [value]");
            }
        }

        protected void HandleStep()
        {
            uint time = cpu.Execute();
            writer.WriteLine("[Execute took {0} ticks]", time);
        }

        protected void HandleUSP(string[] tokens)
        {
            if (tokens is null)
            {
                throw new ArgumentNullException(nameof(tokens));
            }

            if (tokens.Length == 1)
            {
                writer.WriteLine("USP: {0}", cpu.GetUSP().ToString("x", CultureInfo.InvariantCulture));
            }
            else if (tokens.Length == 2)
            {
                uint value;
                try
                {
                    value = ParseInt(tokens[1]);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Bad value [{tokens[1]}]");
                    return;
                }

                cpu.SetUSP(value);
            }
            else
            {
                writer.WriteLine($"usage: {tokens[0]} [value]");
            }
        }

        protected string MakeFlagView()
        {
            StringBuilder sb = new StringBuilder(5);
            sb.Append((cpu.IsFlagSet(cpu.XFlag) ? 'X' : '-'));
            sb.Append((cpu.IsFlagSet(cpu.NFlag) ? 'N' : '-'));
            sb.Append((cpu.IsFlagSet(cpu.ZFlag) ? 'Z' : '-'));
            sb.Append((cpu.IsFlagSet(cpu.VFlag) ? 'V' : '-'));
            sb.Append((cpu.IsFlagSet(cpu.CFlag) ? 'C' : '-'));
            return sb.ToString();
        }

        protected uint ParseInt(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            uint v;
            if (value.StartsWith("$", StringComparison.InvariantCulture))
            {
                try
                {
                    v = (uint)(long.Parse(value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) & 4294967295L);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Not a valid hex number [{value}]");
                    throw;
                }
            }
            else
            {
                try
                {
                    v = (uint)(long.Parse(value, CultureInfo.InvariantCulture) & 4294967295L);
                }
                catch (FormatException)
                {
                    writer.WriteLine($"Not a valid decimal number [{value}]");
                    throw;
                }
            }

            return v;
        }

        protected void ShowHelp()
        {
            writer.WriteLine("Command Help:");
            writer.WriteLine("Addresses and values can be specified in hexadecimal by preceeding the value with '$'");
            writer.WriteLine("      eg. d0 $deadbeef  - Set register d0 to 0xDEADBEEF");
            writer.WriteLine("          m $10         - Memory dump starting at 0x10 (16 in decimal)");
            writer.WriteLine("          pc 10         - Set the PC register to 10 (0x0A in hexadecimal)");
            writer.WriteLine("General:");
            writer.WriteLine("  ?,h,help              - Show this help.");
            writer.WriteLine("  q                     - Quit.");
            writer.WriteLine("Registers:");
            writer.WriteLine("  r                     - Display all registers");
            writer.WriteLine("  d[0-9] [value]        - Set or view a data register");
            writer.WriteLine("  a[0-9] [value]        - Set or view an address register");
            writer.WriteLine("  pc [value]            - Set or view the PC register");
            writer.WriteLine("  sr [value]            - Set or view the SR register");
            writer.WriteLine("  ccr [value]           - Set or view the CCR register");
            writer.WriteLine("  usp [value]           - Set or view the USP register");
            writer.WriteLine("  ssp [value]           - Set or view the SSP register");
            writer.WriteLine("Memory:");
            writer.WriteLine("  m <address>           - View (128 byte) memory dump starting at the specified address");
            writer.WriteLine("  mb <address> [value]  - Set or view a byte (8-bit) value at the specified address");
            writer.WriteLine("  mw <address> [value]  - Set or view a word (16-bit) value at the specified address");
            writer.WriteLine("  ml <address> [value]  - Set or view a long (32-bit) value at the specified address");
            writer.WriteLine("  load <address> <file> - Load <file> into memory starting at <address>");
            writer.WriteLine("Execution & Disassembly:");
            writer.WriteLine("  s                     - Execute the instruction at the PC register");
            writer.WriteLine("  d <address> [count]   - Disassemble the memory starting at <address> for an optional");
            writer.WriteLine("                          <count> instructions. Default is 8 instructions.");
        }
    }
}