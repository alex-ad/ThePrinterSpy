using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Props = ThePrinterSpyControl.Properties.Settings;

namespace ThePrinterSpyControl.Models
{
    public class ReportDateConfig : INotifyPropertyChanged
    {
        private DateTime _start;
        private DateTime _end;
        private PeriodType _period;
        private byte _periodIndex;
        private bool _isEnabled;

        public enum PeriodType : byte
        {
            FromDateToDate = 0,
            FromDateToNow,
            CurrentDay,
            CurrentWeek,
            CurrentMonth,
            CurrentYear,
            PreviousDay,
            PreviousWeek,
            PreviousMonth,
            PreviousYear,
            Last7Days,
            Last30Days,
            Last180Days
        }

        public DateTime Start
        {
            get => _start;
            set
            {
                if (value == _start) return;
                SetStart(value);
                OnPropertyChanged();
            }
        }

        public DateTime End
        {
            get => _end;
            set
            {
                if (value == _end) return;
                SetEnd(value);
                OnPropertyChanged();
            }
        }

        public PeriodType Period
        {
            get => _period;
            set
            {
                if (value == _period) return;
                _period = value;
                OnPropertyChanged();
            }
        }

        public byte PeriodIndex
        {
            get => _periodIndex;
            set
            {
                if (value == _periodIndex) return;
                _periodIndex = value;
                _period = (PeriodType)_periodIndex;
                SetEnd(DateTime.Today);
                SetStart(DateTime.Today);
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

        public Dictionary<PeriodType, string> TypeList { get; } = new Dictionary<PeriodType, string>
        {
            { PeriodType.FromDateToDate, "Заданный период (дата-дата)" },
            { PeriodType.FromDateToNow, "От указанной даты по текущий день" },
            { PeriodType.CurrentDay, "Текущий день" },
            { PeriodType.CurrentWeek, "Текущая неделя" },
            { PeriodType.CurrentMonth, "Текущий месяц" },
            { PeriodType.CurrentYear, "Текущий год" },
            { PeriodType.PreviousDay, "Предыдущий день" },
            { PeriodType.PreviousWeek, "Предыдущая неделя" },
            { PeriodType.PreviousMonth, "Предыдущий месяц" },
            { PeriodType.PreviousYear, "Предыдущий год" },
            { PeriodType.Last7Days, "Последние 7 дней" },
            { PeriodType.Last30Days, "Последние 30 дней" },
            { PeriodType.Last180Days, "Последние 180 дней" }
        };

        public ReportDateConfig()
        {
            try
            {
                _period = (PeriodType)Props.Default.ReportPeriod;
                _periodIndex = Props.Default.ReportPeriod;
                SetEnd(Props.Default.ReportEnd);
                SetStart(Props.Default.ReportStart);
                _isEnabled = Props.Default.ReportPeriodEnabled;
            }
            catch
            {
                _period = PeriodType.CurrentMonth;
                _periodIndex = 4;
                SetEnd(DateTime.Today);
                SetStart(DateTime.Today);
                _isEnabled = true;
            }
            finally
            {
                OnPropertyChanged();
            }
        }

        public override string ToString() => $"{_start.ToString("dd.MM.yyyy")} - {_end.ToString("dd.MM.yyyy")}";

        //public bool IsInPeriod(DateTime date) => (date.CompareTo(_start) > -1 && date.CompareTo(_end) < 1);

        private void SetStart(DateTime date)
        {
            if ((_period == PeriodType.FromDateToDate) || (_period == PeriodType.FromDateToNow)) _start = date;
            else if (_period == PeriodType.CurrentDay) _start = DateTime.Today;
            else if (_period == PeriodType.CurrentWeek) _start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            else if (_period == PeriodType.CurrentMonth) _start = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
            else if (_period == PeriodType.CurrentYear) _start = DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1);
            else if (_period == PeriodType.PreviousDay) _start = DateTime.Today.AddDays(-1);
            else if (_period == PeriodType.PreviousWeek) _start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1 - 7);
            else if (_period == PeriodType.PreviousMonth) _start = DateTime.Today.AddDays(-DateTime.Today.Day + 1).AddMonths(-1);
            else if (_period == PeriodType.PreviousYear) _start = DateTime.Today.AddDays(-DateTime.Today.DayOfYear + 1).AddYears(-1);
            else if (_period == PeriodType.Last7Days) _start = DateTime.Today.AddDays(-7 + 1);
            else if (_period == PeriodType.Last30Days) _start = DateTime.Today.AddDays(-30 + 1);
            else _start = DateTime.Today.AddDays(-180 + 1);
            if (date.CompareTo(_end) > 0) _start = _end;
        }

        private void SetEnd(DateTime date)
        {
            if (_period == PeriodType.FromDateToDate) _end = date;
            else if ((_period == PeriodType.FromDateToNow)
                || (_period == PeriodType.CurrentDay)
                || (_period == PeriodType.CurrentWeek)
                || (_period == PeriodType.CurrentMonth)
                || (_period == PeriodType.CurrentYear)
                || (_period == PeriodType.Last7Days)
                || (_period == PeriodType.Last30Days)
                || (_period == PeriodType.Last180Days)
                ) _end = DateTime.Today;
            else if (_period == PeriodType.PreviousDay) _end = DateTime.Today.AddDays(-1);
            else if (_period == PeriodType.PreviousWeek) _end = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            else if (_period == PeriodType.PreviousMonth) _end = DateTime.Today.AddDays(-DateTime.Today.Day);
            else if (_period == PeriodType.PreviousYear) _end = DateTime.Today.AddDays(-DateTime.Today.DayOfYear);
            if (date.CompareTo(_start) < 0) _end = _start;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
