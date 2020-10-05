﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport.ExtraMoneyDetailRecordReport
{
    public class ExtraMoneyDetailRecordReport : ObservableObject
    {
        public ExtraMoneyDetailRecordReport(DataRow r) {
            Note = r.Field<string>("CashFlow_Note");
            Value = Math.Round(r.Field<decimal>("CashFlow_Value"), 0);
        }
        public string Note { get; set; }
        public decimal Value { get; set; }
    }
}
