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
        public AdjustPharmacistViewModel()
        {
            MonthViewCalendar = new MonthViewCalendar();
        }
    }
}
