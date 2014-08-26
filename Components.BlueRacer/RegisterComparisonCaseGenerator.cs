using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class RegisterComparisonCaseGenerator
    {
        public static string Generate(InstructionSubOpcode opcode, string op)
        {
            var subCases = Enumerable
                .Range(0, Register.Count)
                .Select(x => 
                    "case (`Operand8_2) " +
                    Enumerable
                        .Range(0, Register.Count)
                        .Select(y => string.Format(
                            "{1}: begin _isEq <= _r{0} {2} _r{1}; end ", 
                            x, 
                            y, 
                            op))
                        .Join() + 
                    " default: begin `SetError(`Error_Opcode) end endcase")
                .ToArray();

            var cases = Enumerable
                .Range(0, Register.Count)
                .Select(x => string.Format("{0}: begin {1} end ", x, subCases[x]))
                .Concat(new [] { "default: begin `SetError(`Error_Opcode) end " })
                .Join(" ");

            var stmt = string.Format(
                "`{0}: begin case (`Operand8_1) {1} endcase AdvancePC(); end ",
                opcode,
                cases);

            return stmt;
        }
    }
}
