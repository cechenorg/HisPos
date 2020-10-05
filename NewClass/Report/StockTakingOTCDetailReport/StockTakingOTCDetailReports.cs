using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Report.StockTakingDetailReport
{
   public class StockTakingOTCDetailReports : ObservableCollection<StockTakingOTCDetailReport>
    {
        public StockTakingOTCDetailReports(string typeId, DateTime sDate, DateTime eDate) {
            GetDataByDate(typeId, sDate, eDate);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate) {
            DataTable table = StockTakingOTCDetailReportDb.GetDataByDate(typeId,sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows) {
                Add(new StockTakingOTCDetailReport(r));
            }
        }
    }
}
