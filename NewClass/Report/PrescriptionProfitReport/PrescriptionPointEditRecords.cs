using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionProfitReport
{
    public class PrescriptionPointEditRecords : Collection<PrescriptionPointEditRecord>
    {
        public PrescriptionPointEditRecords()
        {
        }

        public void GetEditRecords(DateTime sDate, DateTime eDate)
        {
            Clear();
            var recordsTable = PrescriptionProfitReportDb.GetPrescriptionPointEditRecordsByDates(sDate, eDate);
            foreach (DataRow r in recordsTable.Rows)
            {
                Add(new PrescriptionPointEditRecord(r));
            }
        }
    }
}