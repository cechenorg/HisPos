using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport
{
    public class TradeProfitDetailEmpReport : ObservableObject
    {
        public TradeProfitDetailEmpReport(DataRow r) {
            TraMas_Cashier = r.Field<string>("TraMas_Cashier");
            Emp_Name = r.Field<string>("Emp_Name");
            Profit = r.Field<int>("Profit");
        }

        public string TraMas_Cashier { get; set; }
        public string Emp_Name { get; set; }
        public int Profit { get; set; }
    }
}
