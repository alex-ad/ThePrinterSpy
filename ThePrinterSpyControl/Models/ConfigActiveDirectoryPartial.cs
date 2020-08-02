using System;
using System.ComponentModel;
using ThePrinterSpyControl.Validators;

namespace ThePrinterSpyControl.Models
{
    public partial class ConfigActiveDirectory : ConfigValidator, IDataErrorInfo
    {
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Server):
                        ClearErrors(nameof(Server));
                        if (IsEnabled) AddErrors(nameof(Server), GetErrorsFromAnnotations(nameof(Server), Server));
                        break;
                    case nameof(User):
                        ClearErrors(nameof(User));
                        if (IsEnabled) AddErrors(nameof(User), GetErrorsFromAnnotations(nameof(User), User));
                        break;
                    /*case nameof(Password):
                        ClearErrors(nameof(Password));
                        if (IsEnabled) AddErrors(nameof(Password), GetErrorsFromAnnotations(nameof(Password), Password));
                        break;*/
                }

                return string.Empty;
            }
        }
    }
}
