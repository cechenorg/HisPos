using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashDetailReport
{
    public class StockTakingDetailReport : ObservableObject
    {
        public StockTakingDetailReport(DataRow r) {
            Id = r.Field<string>("ID");
            ChineseName = r.Field<string>("Pro_ChineseName");
            OldValue = r.Field<double>("StoTakDet_OldValue");
            NewValue = r.Field<double>("StoTakDet_NewValue");
            Price = Math.Round(r.Field<decimal>("Price"), 2);
        }
        public string Id { get; set; }
        public string ChineseName { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public decimal Price { get; set; }

    }
}
