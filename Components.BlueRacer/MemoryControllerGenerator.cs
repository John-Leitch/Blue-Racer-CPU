using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class MemoryControllerGenerator
    {
        private VerilogDocument _document = new VerilogDocument();

        private const string _stageTemplate = @"
if (_read{Size}State == 2)
       begin
         read{Size}Buffer[15:8] <= _dataOut;
         _read{Size}State <= 3;
         _readAddressInternal <= readAddress;
       end
";

        private const string _template = @"
task Read{Size};
  begin

     if (read{Size} & _read{Size}State == 0)
       begin
         _readAddressInternal <= readAddress;
         _readEnable <= 1;
         _read{Size}State <= 1;
       end
       
     if (_read{Size}State == 1) //  Wait for data
       begin           
         _read{Size}State <= 2;
         _readAddressInternal <= _readAddressInternal + 1;
       end
       
   if (_read{Size}State == {FinalStage})
       begin
         read{Size}Buffer[7:0] <= _dataOut;
         
         if (read{Size})
            begin
               _read{Size}State <= 2;
               _readAddressInternal <= _readAddressInternal + 1;
            end
         else
            begin
               _read{Size}State <= 0;
            end
       end  
     
  end
endtask

";
    }
}
