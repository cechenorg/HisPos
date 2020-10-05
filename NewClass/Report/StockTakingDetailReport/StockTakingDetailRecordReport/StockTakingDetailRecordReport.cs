﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport
{
    public class StockTakingDetailRecordReport : ObservableObject
    {
        public StockTakingDetailRecordReport(DataRow r) {
            MasterID = r.Field<string>("StoTakDet_MasterID");
            OldValue = r.Field<double>("StoTakDet_OldValue");
            NewValue = r.Field<double>("StoTakDet_NewValue");
            ValueDiff = Math.Round(r.Field<decimal>("StoTakDet_ValueDiff"), 2);
        }
        public string MasterID { get; set; }
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public decimal ValueDiff { get; set; }
    }
}
