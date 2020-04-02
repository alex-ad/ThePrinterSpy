using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Exceptions
{
    class ThePrinterSpyException : ApplicationException
    {
        public DateTime ErrorTimeStamp { get; set; }
        public string ErrorTimeString { get; set; }
        public string CauseOfError { get; set; }

        public ThePrinterSpyException() { }
        public ThePrinterSpyException(string message, string causeOfError) : base(message)
        {
            CauseOfError = causeOfError;
            ErrorTimeStamp = DateTime.Now;
            ErrorTimeString = ErrorTimeStamp.ToString("HH:mm:ss");
        }
        public ThePrinterSpyException(string message, string causeOfError, DateTime errorTimeStamp) : base (message)
        {
            CauseOfError = causeOfError;
            ErrorTimeStamp = errorTimeStamp;
            ErrorTimeString = ErrorTimeStamp.ToString("HH:mm:ss");
        }
    }
}
