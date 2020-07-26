using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        if (string.IsNullOrEmpty(Mask)) AddError(nameof(Mask), "Поле должно содержать непустое значение");
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
                if (!b || i < 1) AddError(nameof(Mask), "Поле должно содержать положительное числовое значение, от 1 и больше");
            }
        }
    }
}
