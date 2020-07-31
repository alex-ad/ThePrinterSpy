using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePrinterSpyControl.Models
{
    class LastError
    {
        private static bool _success;
        private static string _message;

        public static bool Success => _success;
        public static string Message => _message;

        public static void Set(string message, bool success = false)
        {
            _success = success;
            _message = message;
        }
    }
}
