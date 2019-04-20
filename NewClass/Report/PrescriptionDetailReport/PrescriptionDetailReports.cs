using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport
{
    public class PrescriptionDetailReports : ObservableCollection<PrescriptionDetailReport>
    {
        public PrescriptionDetailReports() {
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate) {
            Clear();
            DataTable table = PrescriptionDetailReportDb.GetDataByDate(typeId,sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new PrescriptionDetailReport(r));
            }
        }
    }
}
