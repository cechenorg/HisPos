using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.TradeProfitDetailRecordReport
{
    public class TradeProfitDetailRecordReport : ObservableObject
    {
        public TradeProfitDetailRecordReport(DataRow r) {
            ProductID = r.Field<string>("TraDet_ProductID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            PriceSum = r.Field<int>("TraDet_PriceSum");
        }
        public string ProductID { get; set; }
        public string ChineseName { get; set; }
        public int PriceSum { get; set; }
    }
}
