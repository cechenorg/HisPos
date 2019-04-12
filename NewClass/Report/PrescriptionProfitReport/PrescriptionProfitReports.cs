﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionProfitReports: ObservableCollection<PrescriptionProfitReport>
    {
        public PrescriptionProfitReports() {
        }
        public void GetDataByDate(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = PrescriptionProfitReportDb.GetDataByDate(sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionProfitReport(r));
            }
        }
    }
}
