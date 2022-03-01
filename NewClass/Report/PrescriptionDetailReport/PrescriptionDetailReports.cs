using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport
{
    public class PrescriptionDetailReports : ObservableCollection<PrescriptionDetailReport>
    {
        public PrescriptionDetailReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }
        public PrescriptionDetailReports(DataTable a)
        {
            GetDataByDate(a);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = PrescriptionDetailReportDb.GetDataByDate(typeId, sDate, eDate);
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionDetailReport(r));
            }
        }
        public void GetDataByDate(DataTable a)
        {
            DataTable table = a;
            foreach (DataRow r in table.Rows)
            {
                Add(new PrescriptionDetailReport(r));
            }
        }
    }
}