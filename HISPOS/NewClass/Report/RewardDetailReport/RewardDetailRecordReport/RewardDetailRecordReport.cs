using GalaSoft.MvvmLight;
using System;
using System.Data;
using System.Globalization;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.RewardDetailRecordReport
{
    public class RewardDetailRecordReport : ObservableObject
    {
        public RewardDetailRecordReport(DataRow r)
        {
            MasterID = r.Field<int>("TraMas_ID");
            ProductID = r.Field<string>("TraDet_ProductID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            RewardAmount = r.Field<double>("RewardAmount");
            RewardAmount = Math.Ceiling(RewardAmount);
            DateTime dt = r.Field<DateTime>("ChkoutTime");
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            ChkoutTime = dt.ToString("yyy/MM/dd", culture);
            if (r.Table.Columns.Contains("Pro_RewardPercent"))
            {
                RewardPercent = Convert.ToDouble(r["Pro_RewardPercent"]);
            }
            if (r.Table.Columns.Contains("TraDet_Amount"))
            {
                TraDetAmount = Convert.ToInt32(r["TraDet_Amount"]);
            }
        }

        public string ProductID { get; set; }
        public string ChineseName { get; set; }
        public double RewardAmount { get; set; }
        public string ChkoutTime { get; set; }
        public int MasterID { get; set; }
        public double RewardPercent { get; set; }
        public int TraDetAmount { get; set; }
    }
}