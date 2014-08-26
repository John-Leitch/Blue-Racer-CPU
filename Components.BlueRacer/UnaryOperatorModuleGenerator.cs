using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class UnaryOperatorModuleGenerator
    {
        private const string _template = @"
task {TaskName}Register;
  begin
    case (`Operand8_{Operand})
{Cases}
      default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end
    endcase 
    AdvancePC();
  end
endtask
";
        private const string _caseTemplate = "      {Register}: _r{Register} <= _r{Register} {Operation};";

        private static string CreateCases(string taskname, string op)
        {
            return Enumerable
                .Range(0, Register.Count)
                .Select(x => new StringTemplate(_caseTemplate).PopulateObj(new
                {
                    Register = x,
                    Operation = op,                    
                }))
                .Join("\r\n");
        }

        public static string Generate(string taskName, string op, bool subOpcode)
        {
            return new StringTemplate(_template).PopulateObj(new
            {
                TaskName = taskName,
                Cases = CreateCases(taskName, op),
                Operand = subOpcode ? 1 : 0,
            });            
        }

        public static string Generate(string[][] modules, bool subOpcode)
        {
            return modules.Select(x => Generate(x[0], x[1], subOpcode)).Join("\r\n");
        }
    }
}
