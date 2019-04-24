using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class AdjustPharmacistViewModel : ViewModelBase
    {
        private MonthViewCalendar monthViewCalendar;
        public MonthViewCalendar MonthViewCalendar
        {
            get => monthViewCalendar;
            set
            {
                Set(() => MonthViewCalendar, ref monthViewCalendar, value);
            }
        }

        public string DeclareMonth => (CurrentDate.Year - 1911) + "年" + CurrentDate.Month + "月";
        public static DateTime CurrentDate { get; set; }
        private static DateTime first { get; set; }
        private static DateTime last { get; set; }
        private static DateTime _selectedDate;
        public static DateTime MySelectedDate
        {
            get => _selectedDate;
            set
            {
                var temp = _selectedDate;
                if (value >= first && value <= last)
                {
                    _selectedDate = value;
                }
                else
                {
                    _selectedDate = temp;
                }
            }
        }
        private DateTime _displayDate;
        public DateTime MyDisplayDate
        {
            get => _displayDate;
            set
            {
                if (value >= first && value <= last)
                {
                    _displayDate = value;
                }
                else
                {
                    _displayDate = CurrentDate;
                }
            }
        }
        public AdjustPharmacistViewModel(DateTime declare)
        {
            CurrentDate = declare;
            MyDisplayDate = declare;
            first = new DateTime(declare.AddMonths(1).Year, declare.Month, 1);
            last = new DateTime(declare.AddMonths(1).Year, declare.AddMonths(1).Month, 1).AddDays(-1);
            MonthViewCalendar = new MonthViewCalendar(declare);
        }
    }
}
