using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport
{
    public class TradeProfitDetailReport : ObservableObject
    {
        public TradeProfitDetailReport(DataRow r) {
            Id = r.Field<int>("TraMas_ID");
            Name = r.Field<string>("PosCus_Name");
            RealTotal = r.Field<int>("TraMas_RealTotal");
            ValueDifference = Math.Round(r.Field<decimal>("ValueDifference"), 2);
            Profit = r.Field<int>("Profit");
            CashAmount = r.Field<int>("CashAmount");
            CardAmount = r.Field<int>("CardAmount");
            DiscountAmt = r.Field<int>("DiscountAmt");
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RealTotal { get; set; }
        public decimal ValueDifference { get; set; }
        public int Profit { get; set; }
        public int CashAmount { get; set; }
        public int CardAmount { get; set; }
        public int DiscountAmt { get; set; }

    }
}
