using Components;
using Components.BlueRacer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class BinaryOperatorConstantModuleGenerator
    {
        private const string setATemplate = @"              {0}: {1}A <= _r{0};";        
        private const string setCTemplate = @"              {0}: _r{0} <= {1}C;";

        private static string CreateACases(string name, int count)
        {
            return string.Join("\r\n", Enumerable.Range(0, count).Select(x => string.Format(setATemplate, x, name)));
        }

        private static string CreateCCases(string name, int count)
        {
            return string.Join("\r\n", Enumerable.Range(0, count).Select(x => string.Format(setCTemplate, x, name)));
        }

        #region Template
        private const string registerConstantTemplate = @"
  
task {TaskName}R{Register}Constant;
  begin      
    case ({Module}Stage)
      0:
        begin
          {Module}Stage <= 1;
          {Module}A <= _r{Register};
          {Module}B <= `Operand32;
          {ReadEnable}
        end
{WaitCases}
      {FinalStage}:
        begin      
          {Module}Stage <= 0;
          _r{Register} <= {Module}C;          
          AdvancePC();
        end          
    endcase      
  end
endtask
";
        #endregion

        private const string _waitTemplate = @"      {1}: {0}Stage <= {2};";

        private const string _waitLastTemplate = @"
        {1}:
          begin
            {0}Stage <= {2};
            readInstructionEnable <= 1;
          end
";

        private const string _opcodeCaseTemplate = "`{0}_R{1}_C: {2}R{1}Constant();";

        public static string Generate(string moduleName, string taskName, int registerCount, int latency)
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

            var tasks = Enumerable
                .Range(0, registerCount)
                .Select(x => new StringTemplate(registerConstantTemplate).PopulateObj(new
                {
                    TaskName = taskName,
                    Module = moduleName,
                    Register = x,
                    FinalStage = finalStage,
                    WaitCases = waitCases,
                    ReadEnable = latency == 0 ? "readInstructionEnable <= 1;" : "",
                }))
                .Join("\r\n");

            var doc = new VerilogDocument();

            var cases = Enumerable
                .Range(0, registerCount)
                .Select(x => string.Format(
                    _opcodeCaseTemplate, 
                    taskName.Length > 3 && !taskName.Contains("Shift") ? taskName.Remove(3) : taskName, 
                    x,
                    taskName))
                .Join("\r\n");

            VerilogGlobals.Globals.AddDefineString(taskName + "Cases", cases);

            //doc.AddDefineString(taskName + "Cases", cases);

            //return tasks + "\r\n" + doc.ToString();
            return tasks;
        }

        public static string Generate(object[][] names, int registerCount)
        {
            return names.Select(x => Generate((string)x[0], (string)x[1], registerCount, (int)x[2])).Join("\r\n");
        }
    }
}
