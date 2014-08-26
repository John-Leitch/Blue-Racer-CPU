using Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System.Text.RegularExpressions;

namespace Processor
{
    class Program
    {
        public static VerilogDocument Globals = new VerilogDocument();

        static void GenerateOpcodeEnums()
        {
            byte addStart = 0x10;
            byte subStart = (byte)(addStart + Register.Count);
            byte mulStart = (byte)(subStart + Register.Count);
            byte divStart = (byte)(mulStart + Register.Count);

            byte andStart = (byte)(divStart + Register.Count);
            byte orStart = (byte)(andStart + Register.Count);

            byte assignStart = (byte)(orStart + Register.Count);

            byte eqStart = (byte)(assignStart + Register.Count);
            byte notEqStart = (byte)(eqStart + Register.Count);
            byte ltStart = (byte)(notEqStart + Register.Count);
            byte ltEqStart = (byte)(ltStart + Register.Count);
            byte gtStart = (byte)(ltEqStart + Register.Count);
            byte gtEqStart = (byte)(gtStart + Register.Count);

            var templates = new[]
            {
                new RegisterOpcodeTemplate("Assign_R{0}_Const", assignStart),
                new RegisterOpcodeTemplate("Eq_R{0}_C", eqStart),
                new RegisterOpcodeTemplate("NotEq_R{0}_C", notEqStart),
                new RegisterOpcodeTemplate("LessThan_R{0}_C", ltStart),
                new RegisterOpcodeTemplate("LessThanEq_R{0}_C", ltEqStart),
                new RegisterOpcodeTemplate("GreaterThan_R{0}_C", gtStart),
                new RegisterOpcodeTemplate("GreaterThanEq_R{0}_C", gtEqStart),
                new RegisterOpcodeTemplate("Add_R{0}_C", addStart),
                new RegisterOpcodeTemplate("Sub_R{0}_C", subStart),
                new RegisterOpcodeTemplate("Mul_R{0}_C", mulStart),
                new RegisterOpcodeTemplate("Div_R{0}_C", divStart),
                new RegisterOpcodeTemplate("And_R{0}_C", andStart),
                new RegisterOpcodeTemplate("Or_R{0}_C", orStart),
            };

            var enumStr = InstructionEnumGenerator.Generate<InstructionOpcodeInternal>(
                "InstructionOpcode",
                templates);

            File.WriteAllText(CodeFile.OpcodeEnum, enumStr);

            enumStr = InstructionEnumGenerator.Generate<InstructionSubOpcodeInternal>(
                "InstructionSubOpcode",
                new RegisterOpcodeTemplate[0]);

            File.WriteAllText(CodeFile.SubOpcodeEnum, enumStr);

            enumStr = InstructionEnumGenerator.Generate<InstructionMultiStageSubOpcodeInternal>(
                "InstructionMultiStageSubOpcode",
                new RegisterOpcodeTemplate[0]);

            File.WriteAllText(CodeFile.MultiStageSubOpcodeEnum, enumStr);
        }

        static void GenerateOpcodes()
        {
            var doc = new VerilogDocument();

            foreach (var opcode in (InstructionOpcode[])Enum.GetValues(typeof(InstructionOpcode)))
            {
                doc.AddDefine8(opcode.ToString(), (byte)opcode);
            }

            doc.AddLine();

            foreach (var opcode in (InstructionSubOpcode[])Enum.GetValues(typeof(InstructionSubOpcode)))
            {
                doc.AddDefine8(opcode.ToString(), (byte)opcode);
            }

            doc.AddLine();

            foreach (var opcode in (InstructionMultiStageSubOpcode[])Enum.GetValues(typeof(InstructionMultiStageSubOpcode)))
            {
                doc.AddDefine8(opcode.ToString(), (byte)opcode);
            }

            File.WriteAllText(CodeFile.Opcode, doc.ToString());
        }

        static void GenerateRom()
        {
            var program = File.ReadAllText(CodeFile.Program);
            var machineCode = new AphidAssembler().Assemble(program);
            var bytes = machineCode.Select(x => ByteHelper.ToHex(x)).JoinLines();
            File.WriteAllText(CodeFile.Program + ".txt", bytes);
            File.WriteAllText(CodeFile.Program + "2.txt", bytes.Replace("\r\n", ""));
            File.WriteAllBytes(CodeFile.Program + ".bin", machineCode.SelectMany(x => x).ToArray());
            var programSize = machineCode.Sum(x => x.Count());
            var padding = 5 - (programSize % 5);

            if (padding == 5)
            {
                padding = 0;
            }

            programSize += padding;

            if (programSize > 0xffff)
            {
                throw new InvalidOperationException();
            }

            var asm = new AphidAssembler().AssembleToString(program);

            var doc3 = new VerilogDocument();
            doc3.AddDefineString("Program", asm);
            doc3.AddDefine16("ProgramSize", (ushort)programSize);
            File.WriteAllText(CodeFile.Rom, doc3.ToString());
        }

        static void GenerateMultiStageCases()
        {
            var multistagePatterns = new[]
            {
                "Add",
                "Sub",
                "Mul",
                "Div",
                "Goto",
                "Call",
                "Push",
                "ExtMultiStage",
                "And",
                "Or",
            };

            var cases = EnumHelper
                .GetValues<InstructionOpcode>()
                .Where(x => multistagePatterns.Any(y => x.ToString().Contains(y)))
                .Select(x => "`" + x.ToString())
                .Join(", ");

            Globals.AddDefineString("MultiStageOpcodes", cases);
        }

        static void GenerateArithmeticModules()
        {
            byte divLatency = 30;
            Globals.AddDefine8("DivideLatency", divLatency);

            var arithmeticOps = new[]
	        {
		        new object[] { "adder", "Add", 0 },
		        new object[] { "subtracter", "Subtract", 0 },
		        new object[] { "multiplier", "Multiply", 0 },
		        new object[] { "divider", "Divide", (int)divLatency },
                new object[] { "ander", "And", 0 },
		        new object[] { "orer", "Or", 0 },
	        };

            var arithmeticModules = BinaryOperatorRegisterModuleGenerator.Generate(arithmeticOps)
                + "\r\n" +
                BinaryOperatorConstantModuleGenerator.Generate(arithmeticOps, Register.Count)
                ;

            var tasks = BinaryOperatorConstantModuleGenerator.Generate(
                arithmeticOps, 
                Register.Count);

            arithmeticModules += "\r\n" +
                UnaryOperatorModuleGenerator.Generate(new[]
                {
                    new[] { "Increment", "+ 1" },
                    new[] { "Decrement", "- 1" },
                }, true);

            var doc2 = new VerilogDocument();
            doc2.AddDefineString("ArithmeticModules", arithmeticModules);
            File.WriteAllText(CodeFile.Arithmetic, doc2.ToString());
        }

        static string GenerateReadRegisterCases()
        {
            return Enumerable
                .Range(0, Register.Count)
                .SelectMany(x => new[]
                {
                    string.Format("{0}: read4Address <= _r{0};", x),
                    string.Format("{1}: read4Address <= _r{0} + `Operand8_3;", x, x + Register.Count),
                    string.Format("{1}: read4Address <= _r{0} - `Operand8_3;", x, x + (Register.Count * 2)),
                })
                .Join("\r\n");
        }

        static string GenerateResetRegisters()
        {
            return Enumerable.Range(0, Register.Count).Select(x => string.Format("_r{0} <= 0;", x)).Join(" ");
        }

        static void GenerateEnumUIntDefine<TEnum>(string prefix)
        {
            GenerateEnumDefine<TEnum>(prefix, (x, y) => Globals.AddDefine32(x, (uint)(object)y));            
        }

        static void GenerateEnumByteDefine<TEnum>(string prefix)
        {
            GenerateEnumDefine<TEnum>(prefix, (x, y) => Globals.AddDefine8(x, (byte)(object)y));
        }

        static void GenerateEnumDefine<TEnum>(string prefix, Action<string, TEnum> add)
        {
            var values = EnumHelper.GetValues<TEnum>()
                .Select(x => new
                {
                    Name = string.Format("{0}_{1}", prefix, x.ToString()),
                    Value = x
                });

            foreach (var v in values)
            {
                add(v.Name, v.Value);
            }
        }

        static void GenerateGlobals()
        {
            //uint ramSize = 1024 * 32, pageSize = 16;
            uint ramSize = 1024 * 4, pageSize = 16;

            GenerateEnumUIntDefine<CpuErrorCode>("Error");
            Globals.AddLine();
            GenerateEnumUIntDefine<CpuCommand>("Command");
            Globals.AddLine();
            GenerateEnumByteDefine<CpuCommandState>("CommandState");
            Globals.AddLine();

            Globals.AddDefine32("RamSize", ramSize);
            Globals.AddDefine32("PageSize", pageSize);
            Globals.AddDefine32("Pages", ramSize / pageSize);
            Globals.AddDefineString("RegisterDeclarations", Register.CreateDeclarations());
            Globals.AddDefineString("ReadRegisterCases", GenerateReadRegisterCases());
            Globals.AddDefineString("ResetRegisters", GenerateResetRegisters());

            File.WriteAllText(CodeFile.Globals, Globals.ToString());
        }

        static void VerifyEnumDistinctValues<TEnum>()
        {
            if (EnumHelper.GetValues<TEnum>().Count() !=
                EnumHelper.GetValues<TEnum>().Distinct().Count())
            {
                throw new InvalidOperationException();
            }
        }

        static void ValidateEnums()
        {
            VerifyEnumDistinctValues<CpuCommand>();
            VerifyEnumDistinctValues<CpuErrorCode>();
            VerifyEnumDistinctValues<CpuCommandState>();
            VerifyEnumDistinctValues<InstructionMultiStageSubOpcode>();
            VerifyEnumDistinctValues<InstructionMultiStageSubOpcodeInternal>();
            VerifyEnumDistinctValues<InstructionOpcode>();
            VerifyEnumDistinctValues<InstructionOpcodeInternal>();
            VerifyEnumDistinctValues<InstructionSubOpcode>();
            VerifyEnumDistinctValues<InstructionSubOpcodeInternal>();
        }

        static void Main(string[] args)
        {
            var flag = MemoryAccessFlag.Read | MemoryAccessFlag.Write | MemoryAccessFlag.Execute;
            //var packet = ((uint)flag) << 29;
            var packet = (uint)flag;
            uint pageNumber = 1;

            pageNumber = 0x2;

            if (pageNumber > 0x1FFFFFFF)
            {
                throw new InvalidOperationException();
            }

            packet |= pageNumber;

            ValidateEnums();
            GenerateMultiStageCases();
            GenerateOpcodeEnums();
            GenerateRom();
            GenerateOpcodes();
            GenerateArithmeticModules();
            GenerateGlobals();
            File.Copy(
                @"c:\source\Processor\Processor\CpuCommand.cs",
                @"C:\source\Processor\Programmer\CpuCommand.cs",
                true);

            File.Copy(
                @"c:\source\Processor\Processor\MemoryAccessFlag.cs",
                @"c:\source\Processor\Programmer\MemoryAccessFlag.cs",
                true);
        }
    }
}
