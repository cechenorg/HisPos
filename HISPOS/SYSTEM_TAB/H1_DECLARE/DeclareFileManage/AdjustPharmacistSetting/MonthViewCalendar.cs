using Microsoft.Windows.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using Calendar = Microsoft.Windows.Controls.Calendar;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    public class MonthViewCalendar : Calendar, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        static MonthViewCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthViewCalendar), new FrameworkPropertyMetadata(typeof(MonthViewCalendar)));
        }

        public MonthViewCalendar() : base()
        {
        }

        public MonthViewCalendar(DateTime declare) : base()
        {
            DateTime first = new DateTime(declare.AddMonths(1).Year, declare.Month, 1);
            DateTime last = new DateTime(declare.AddMonths(1).Year, declare.AddMonths(1).Month, 1).AddDays(-1);
            CalendarDateRange cdr = new CalendarDateRange(DateTime.MinValue, first.AddDays(-1));
            CalendarDateRange cdr2 = new CalendarDateRange(last.AddDays(1), DateTime.MaxValue);
            BlackoutDates.Add(cdr);
            BlackoutDates.Add(cdr2);
            DisplayDate = declare;
        }
    }
}