using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AdjustPharmacistViewModel : ViewModelBase
    {
        public ObservableCollection<DateTime> _days = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> Days
        {
            get
            {
                return _days;
            }
        }
        private int _currentYear = 2010;
        public int CurrentYear
        {
            get => _currentYear;
            set
            {
                if (_currentYear == value || value <= 1978 || value >= 9999) return;
                Set(() => CurrentYear, ref _currentYear, value);
                SetCalendar(_currentYear, CurrentMonth);
            }
        }

        private int _currentMonth = 1;
        public int CurrentMonth
        {
            get => _currentMonth;
            set
            {
                if (_currentMonth == value || value >= 13 || value <= 0) return;
                Set(() => CurrentMonth, ref _currentMonth, value);
                SetCalendar(CurrentYear, _currentMonth);
            }
        }
        public AdjustPharmacistViewModel(DateTime declareDate)
        {

        }
        private void SetCalendar(int year, int month)
        {
            _days.Clear();
            DateTime datetime = new DateTime(year, month, 1);
            int week = (int)datetime.DayOfWeek;
            datetime = datetime.AddDays(1 - week);
            for (int i = 0; i < 42; i++)
            {

                _days.Add(datetime.AddDays(i));
            }
        }
        private void ChooseDate(object obj)
        {
            DateTime dt;
            DateTime.TryParse(obj.ToString(), out dt);
            CurrentMonth = dt.Month;
            CurrentYear = dt.Year;
        }
    }
}
