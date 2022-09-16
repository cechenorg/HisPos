using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport
{
    public class StockTakingDetailRecordReport : ObservableObject
    {
       
        public string ID { get; set; }

        public string Pro_ChineseName { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public decimal ValueDiff { get; set; }
    }
}