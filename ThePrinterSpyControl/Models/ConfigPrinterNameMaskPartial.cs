using System;
using System.ComponentModel;
using ThePrinterSpyControl.Properties;
using ThePrinterSpyControl.Validators;

namespace ThePrinterSpyControl.Models
{
    public partial class ConfigPrinterNameMask : ConfigValidator, IDataErrorInfo
    {
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Mask))
                {
                    ClearErrors(nameof(Mask));
                    if (IsEnabled)
                    {
                        if (string.IsNullOrEmpty(Mask)) AddError(nameof(Mask), Resources.ConfigPrinterNameValueError);
                        else CheckIntType(Type);
                    }
                }

                return string.Empty;
            }
        }

        private void CheckIntType(MaskType type)
        {
            if (type == MaskType.BeginName || type == MaskType.EndName)
            {
                var b = int.TryParse(Mask, out int i);
                if (!b || i < 1) AddError(nameof(Mask), Resources.ConfigPrinterNameValueError);
            }
        }
    }
}
