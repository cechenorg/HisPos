using GalaSoft.MvvmLight;
using System;
using System.Data;

namespace His_Pos.NewClass.Report.ExtraMoneyDetailReport
{
    public class ExtraMoneyDetailReport : ObservableObject
    {
        public ExtraMoneyDetailReport(DataRow r)
        {
            Name = r.Field<string>("CashFlow_Name");
            Value = Math.Round(r.Field<decimal>("CashFlow_Value"), 0);
            CashFlow_Case = r.Field<string>("CashFlow_Case");
        }

        public string Name { get; set; }
        public decimal Value { get; set; }
        public string CashFlow_Case { get; set; }
    }
}