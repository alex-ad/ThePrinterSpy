using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Models
{
    public class PrinterNameMaskConfig : INotifyPropertyChanged
    {
        private MaskType _type;
        private string _mask;
        private byte _typeIndex;
        private bool _isEnabled;

        public enum MaskType : byte
        {
            WholeName = 0,
            BeginName,
            EndName,
            ContainsName,
            RegexpName
        }

        public MaskType Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public string Mask
        {
            get => _mask;
            set
            {
                if (value == _mask) return;
                _mask = value;
                OnPropertyChanged();
            }
        }

        public byte TypeIndex
        {
            get => _typeIndex;
            set
            {
                if (value == _typeIndex) return;
                _typeIndex = value;
                _type = (MaskType) _typeIndex;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<MaskType, string> TypeList { get; } = new Dictionary<MaskType, string>
        {
            { MaskType.WholeName, "Оригинальное имя (вся строка)" },
            { MaskType.BeginName, "Количество символов от начала строки" },
            { MaskType.EndName, "Количество символов от конца строки" },
            { MaskType.ContainsName, "Строка содержит символы" },
            { MaskType.RegexpName, "Регулярное выражение" }
        };

        public PrinterNameMaskConfig()
        {
            try
            {
                _type = (MaskType) Props.Default.PrinterNameMaskType;
                _mask = Props.Default.PrinterNameMaskValue;
                _typeIndex = (byte) _type;
                _isEnabled = Props.Default.PrinterNameMaskEnabled;
            }
            catch
            {
                _type = MaskType.WholeName;
                _mask = "";
                _typeIndex = 0;
                _isEnabled = false;
            }
            finally
            {
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
