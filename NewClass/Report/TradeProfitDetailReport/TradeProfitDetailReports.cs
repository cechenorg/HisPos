﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.TradeProfitDetailReport
{
   public class TradeProfitDetailReports : ObservableCollection<TradeProfitDetailReport>
    {
        public TradeProfitDetailReports(string typeId, DateTime sDate, DateTime eDate) {
            GetDataByDate(typeId, sDate, eDate);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = TradeProfitDetailReportDb.GetDataByDate(typeId,sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new TradeProfitDetailReport(r));
            }
        }
    }
}