using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class VerificationResult
    {
        public bool Passed { get; set; }

        public string Message { get; set; }

        public string List { get; set; }

        //public bool HasError { get; set; }

        public CpuErrorCode Error { get; set; }

        public VerificationResult(
            bool passed, 
            string message, 
            string list,
            //bool hasError,
            CpuErrorCode error)
        {
            Passed = passed;
            Message = message;
            List = list;
            //HasError = hasError;
            Error = error;
        }

        public VerificationResult(bool passed, string list, CpuErrorCode error)
            : this(passed, null, list, error)
        {
            
        }
    }
}
