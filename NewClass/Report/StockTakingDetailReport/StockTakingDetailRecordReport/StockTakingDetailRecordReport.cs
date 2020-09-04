using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport
{
    public class StockTakingDetailRecordReport : ObservableObject
    {
        public StockTakingDetailRecordReport(DataRow r) {
            MasterID = r.Field<string>("StoTakDet_MasterID");
            OldValue = r.Field<double>("StoTakDet_OldValue");
            NewValue = r.Field<double>("StoTakDet_NewValue");
            ValueDiff = r.Field<double>("StoTakDet_ValueDiff");
        }
        public string MasterID { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public double ValueDiff { get; set; }
    }
}
