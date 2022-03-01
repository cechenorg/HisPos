using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport
{
    public class TradeProfitDetailRecordReport : ObservableObject
    {
        public TradeProfitDetailRecordReport(DataRow r)
        {
            ProductID = r.Field<string>("TraDet_ProductID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            PriceSum = r.Field<int>("TraDet_PriceSum");
        }

        public string ProductID { get; set; }
        public string ChineseName { get; set; }
        public int PriceSum { get; set; }
    }
}