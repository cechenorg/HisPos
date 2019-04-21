﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashDetailReport.CashDetailRecordReport
{
    public class CashDetailRecordReport: ObservableObject
    {
        public CashDetailRecordReport(DataRow r) {
            Date = r.Field<DateTime>("CashFlow_Time");
            TypeName = r.Field<string>("CashFlow_Name");
            Value = r.Field<double>("CashFlow_Value");
        }
        public DateTime Date { get; set; }
        public string TypeName { get; set; }
        public double Value { get; set; }
    }
}
