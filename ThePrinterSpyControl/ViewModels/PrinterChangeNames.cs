using System;

namespace ThePrinterSpyControl.ViewModels
{
    class PrinterChangeNames
    {
        public string ComputerName { get; }
        public string PrinterOldName { get; }
        public string PrinterNewName { get; }

        public PrinterChangeNames(string computerName, string printerOldName, string printerNewName)
        {
            if ((computerName?.Length < 2) || (printerOldName?.Length < 2) || (printerNewName?.Length < 2))
                throw new ArgumentException("The Computer or Printer name must be greater then 2 symbols");
            if (printerOldName.Equals(printerNewName, StringComparison.InvariantCultureIgnoreCase))
                throw  new ArgumentException("The Printer OldName and NewName must be different");

            ComputerName = computerName;
            PrinterOldName = printerOldName;
            PrinterNewName = printerNewName;
        }
    }
}
