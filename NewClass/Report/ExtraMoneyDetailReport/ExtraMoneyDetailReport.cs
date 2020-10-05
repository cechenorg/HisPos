using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.ExtraMoneyDetailReport
{
    public class ExtraMoneyDetailReport : ObservableObject
    {
        public ExtraMoneyDetailReport(DataRow r) {
            Name = r.Field<string>("CashFlow_Name");
            Value = Math.Round(r.Field<decimal>("CashFlow_Value"), 0);
        }

        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
