using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class BinaryOperatorRegisterModuleGenerator
    {
        private const string setATemplate = @"              {0}: {1}A <= _r{0};";
        private const string setBTemplate = @"              {0}: {1}B <= _r{0};";
        private const string setCTemplate = @"              {0}: _r{0} <= {1}C;";

        private static string CreateACases(string name, int count)
        {
            return string.Join("\r\n", Enumerable.Range(0, count).Select(x => string.Format(setATemplate, x, name)));
        }

        private static string CreateBCases(string name, int count)
        {
            return string.Join("\r\n", Enumerable.Range(0, count).Select(x => string.Format(setBTemplate, x, name)));
        }

        private static string CreateCCases(string name, int count)
        {
            return string.Join("\r\n", Enumerable.Range(0, count).Select(x => string.Format(setCTemplate, x, name)));
        }

        #region Template
        private const string registerRegisterTemplate = @"
  reg [Architecture-1:0] {0}A = 0, {0}B = 0;
  wire [Architecture-1:0] {0}C;
  {5} {0}(.clk(clk), .a({0}A), .b({0}B), .c({0}C));
  reg [7:0] {0}Stage = 0;

  task {1}Registers;
    begin
      
      case ({0}Stage)
        0:
          begin
            {0}Stage <= 1;
            case (`Operand8_1)
{2}
              default: begin {0}A <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end
            endcase
      
            case (`Operand8_2)
{3}
              default: begin {0}B <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end
            endcase

            {8}
          end
{6}
        {7}:
          begin      
            {0}Stage <= 0;      
            case (`Operand8_1)
{4}
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end
            endcase   
            
            AdvancePC();
          end
          
      endcase
      
    end
  endtask
";
        #endregion

        private const string _waitTemplate = @"        {1}: {0}Stage <= {2};",
            _waitLastTemplate = @"
        {1}:
          begin
            {0}Stage <= {2};
            readInstructionEnable <= 1;
          end
";

        public static string Generate(string moduleName, string taskName, int latency)
        {
            string waitCases = "";

            if (latency != 0)
            {
                //Console.WriteLine();

                waitCases = Enumerable
                    .Range(1, latency - 1)
                    .Select(x => string.Format(_waitTemplate, moduleName, x, x + 1))
                    .Join("\r\n");

                waitCases += "\r\n" + string.Format(_waitLastTemplate, moduleName, latency, latency + 1);
            }

            var finalStage = latency + 1;

            var module = string.Format(
                registerRegisterTemplate,
                moduleName,
                taskName,
                CreateACases(moduleName, Register.Count),
                CreateBCases(moduleName, Register.Count),
                CreateCCases(moduleName, Register.Count),
                moduleName[0].ToString().ToUpper() + moduleName.Substring(1),
                waitCases,
                finalStage,
                latency == 0 ? "readInstructionEnable <= 1;" : "");

            return module;
        }

        public static string Generate(object[][] names)
        {
            return names.Select(x => Generate((string)x[0], (string)x[1], (int)x[2])).Join("\r\n");
        }
    }
}
