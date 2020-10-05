﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.ExtraMoneyDetailReport
{
   public class ExtraMoneyDetailReports : ObservableCollection<ExtraMoneyDetailReport>
    {
        public ExtraMoneyDetailReports( DateTime sDate, DateTime eDate) {
            GetDataByDate(sDate, eDate);
        }

        public void GetDataByDate( DateTime sDate, DateTime eDate) {
            DataTable table = ExtraMoneyDetailReportDb.GetDataByDate(sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new ExtraMoneyDetailReport(r));
            }
        }
    }
}
