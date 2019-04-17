using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashDetailReport
{
   public class CashDetailReports : ObservableCollection<CashDetailReport>
    {
        public CashDetailReports() {
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = CashDetailReportDb.GetDataByDate(typeId,sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new CashDetailReport(r));
            }
        }
    }
}
