using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Globalization;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport.TradeProfitDetailEmpRecordReport
{
    public class TradeProfitDetailEmpRecordReport : ObservableObject
    {
        public TradeProfitDetailEmpRecordReport(DataRow r)
        {
            TraMas_ID = 1;

            TraMas_RealTotal = r.Field<int>("TraMas_RealTotal");

            DateTime dt = r.Field<DateTime>("TraMas_ChkoutTime");
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            TraMas_ChkoutTime = dt.ToString("yyy/MM/dd", culture);
        }

        public int TraMas_ID { get; set; }
        public string TraMas_ChkoutTime { get; set; }
        public int TraMas_RealTotal { get; set; }
    }
}