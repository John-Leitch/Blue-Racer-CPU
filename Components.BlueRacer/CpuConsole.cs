using Components.Aphid.Interpreter;
using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using Components.ConsolePlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class CpuConsole
    {
        private CpuEthernetConnection _connection;

        private CpuDebugger _debugger;

        private CpuProgrammer _programmer;

        public void Connect(string host)
        {
            Cli.Write("Connecting... ");
            _connection = CpuEthernetConnection.Create(host);
            _debugger = new CpuDebugger(_connection);
            _programmer = new CpuProgrammer(_connection);
            //var flags = _programmer.GetPageFlags(0x0);
            //_programmer.SetPageFlags(0x0,
            //    MemoryAccessFlag.Execute |
            //    MemoryAccessFlag.Read |
            //    MemoryAccessFlag.Write);
            //var flags2 = _programmer.GetPageFlags(0x0);

            Cli.WriteLine("~Green~Connected~R~");
        }

        public void ReadCommand()
        {
            //var cd = Directory
            //    .GetCurrentDirectory()
            //    .Split('\\')
            //    .Select(x => string.Format("~White~{0}~R~", x))
            //    .Join("~Cyan~\\~R~");

            //Cli.Write("{0}~Cyan~>~R~", cd);
            Cli.Write("~White~#~R~~Cyan~>~R~");
            var cmd = Console.ReadLine();
            var args = ArgLexer.Tokenize(cmd);
            InterpretCommand(args);
        }

        private void PrintError(string message, params object[] args)
        {
            Cli.WriteLine("~Red~{0}~R~", string.Format(message, args));
        }

        private void CheckArguments(string[] tokens, int count)
        {
            if (tokens.Length != count + 1)
            {
                PrintError("Expected {0} argument(s)", count);
                return;
            }
        }

        private T InterpretArg<T>(string arg)
        {
            var tokens = new AphidLexer(arg).GetTokens();
            var parser = new AphidParser(tokens);
            parser.NextToken();

            var varName = "$val";
            var exp = new BinaryOperatorExpression(
                new IdentifierExpression(varName),
                AphidTokenType.AssignmentOperator,
                parser.ParseExpression());

            var interpreter = new AphidInterpreter();
            interpreter.Interpret(new List<Expression> { exp });

            var val = interpreter.CurrentScope[varName].Value;

            if (val == null)
            {
                throw new AphidParserException(new AphidToken(AphidTokenType.nullKeyword, "", 0));
            }

            if (typeof(T) == typeof(uint))
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            else
            {
                return (T)val;
            }
        }

        private void Dump4(string[] tokens)
        {
            CheckArguments(tokens, 1);
            var val = InterpretArg<uint>(tokens[1]);

            for (int line = 0; line < 8; line++)
            {
                Cli.Write("~Gray~{0:X8}:~R~ ", val);

                for (int i = 0; i < 4; i++)
                {
                    Cli.Write("~White~{0:X8}~R~ ", _debugger.Read(val));
                    val += 4;
                }
                Cli.WriteLine();
            }
        }

        private void DumpString(string[] tokens)
        {
            CheckArguments(tokens, 1);
            var val = InterpretArg<uint>(tokens[1]);

            var inString = true;

            var sb = new StringBuilder();

            while (inString)
            {
                var u = BitConverter
                    .GetBytes(_debugger.Read(val))
                    .Reverse();

                foreach (var b in u)
                {
                    if (b != 0 && b < 128)
                    {
                        sb.Append((char)b);
                    }
                    else
                    {
                        inString = false;
                        break;
                    }
                }

                val += 4;
            }

            Cli.WriteLine(sb.ToString());
        }

        private void InterpretCommand(string[] tokens)
        {
            if (tokens.Length == 0)
            {
                return;
            }

            try
            {
                switch (tokens[0])
                {
                    case "p":
                        CheckArguments(tokens, 1);
                        Program(tokens[1]);
                        break;

                    case "r":
                        CheckArguments(tokens, 0);
                        Cli.Dump(_debugger.GetContext());
                        break;

                    case "d4":
                        Dump4(tokens);
                        break;

                    case "ds":
                        DumpString(tokens);
                        break;

                    case "b":
                        Cli.WriteLine("Breaking");
                        _debugger.Break();
                        break;

                    case "g":
                        Cli.WriteLine("Continuing");
                        _debugger.Continue();
                        break;

                    case "restart":
                        Cli.WriteLine("Restarting");
                        _debugger.Restart();
                        break;

                    case "cls":
                        Console.Clear();
                        break;

                    case "cd":
                        CheckArguments(tokens, 1);

                        try
                        {
                            Directory.SetCurrentDirectory(tokens[1]);
                        }
                        catch (Exception e)
                        {
                            Cli.WriteLine("~Red~Error:~R~ {0}", e.Message);
                        }

                        break;

                    default:
                        PrintError("Invalid command {0}", tokens[0]);
                        break;
                }
            }
            catch (AphidParserException e)
            {
                PrintError("Error parsing argument: {0}", e.Message);
            }
        }

        private void Program(string filename)
        {
            if (!File.Exists(filename))
            {
                PrintError("Could not file \"{0}\"", filename);

                return;
            }

            if (Path.GetExtension(filename).ToLower() == ".alx")
            {
                Cli.Write("Assembling... ");
                var binFile = Path.GetRandomFileName();

                try
                {
                    new AphidAssembler().AssembleFileToBin(filename, binFile);
                }
                catch (Exception e)
                {
                    Cli.WriteLine("~Red~Error:~R~ {0}", e);
                    return;
                }

                Cli.WriteLine("~Green~{0:n0}~R~ byte binary created", new FileInfo(binFile).Length);

                Cli.Write("Programming... ");

                try
                {
                    _programmer.Program(binFile);
                    Cli.WriteLine("~Green~done~R~");
                }
                catch (Exception e)
                {
                    Cli.WriteLine("~Red~Error:~R~ {0}", e.Message);
                }

                File.Delete(binFile);
            }
            else
            {
                _programmer.Program(filename);
            }
        }
    }
}
