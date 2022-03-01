using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport
{
    public class TradeProfitDetailEmpReport : ObservableObject
    {
        public TradeProfitDetailEmpReport(DataRow r)
        {
            TraMas_Cashier = r.Field<string>("TraMas_Cashier");
            Emp_Name = r.Field<string>("Emp_Name");
            Profit = r.Field<int>("Profit");
        }

        public string TraMas_Cashier { get; set; }
        public string Emp_Name { get; set; }
        public int Profit { get; set; }
    }
}