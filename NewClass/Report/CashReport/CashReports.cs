using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.CashReport
{
    public class CashReports: ObservableCollection<CashReport>
    {
        public CashReports() { }


        public void GetDataByDate(DateTime sDate,DateTime eDate) {
            Clear();
            DataTable table = CashReportDb.GetDataByDate(sDate,eDate);
            foreach (DataRow r in table.Rows) {
                Add(new CashReport(r));
            }
        }
    }
}
