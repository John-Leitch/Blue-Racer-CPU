using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class McuTestTemplate
    {
        public string ProgramVerilogFile { get; set; }

        public int TestCount { get; set; }

        public string Tests { get; private set; }

        public Dictionary<int, string> InstructionCompleteCases { get; set; }

        public string InstructionCompleteCasesText { get; private set; }

        public override string ToString()
        {
            Tests = Enumerable
                .Range(0, TestCount)
                .Select(x => "test" + x)
                .Join(", ");

            InstructionCompleteCasesText = InstructionCompleteCases
                .Select(x => string.Format(
                    @"
                        {0}: 
                                  begin
                                      {1}
                                  end
                        ",
                    x.Key,
                    x.Value))
                .JoinLines();


            var template = File.ReadAllText(@"C:\altera\13.1\McuTestTemplate.v");

            var t = new StringTemplate(template).PopulateObj(this);
            return t;
        }
    }
}
