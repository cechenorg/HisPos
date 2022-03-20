using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Report.TradeProfitDetailEmpReport
{
    public class TradeProfitDetailEmpReports : ObservableCollection<TradeProfitDetailEmpReport>
    {
        public TradeProfitDetailEmpReports(string typeId, DateTime sDate, DateTime eDate)
        {
            GetDataByDate(typeId, sDate, eDate);
        }
        public TradeProfitDetailEmpReports(DataTable s)
        {
            GetDataByDate(s);
        }

        public void GetDataByDate(string typeId, DateTime sDate, DateTime eDate)
        {
            DataTable table = TradeProfitDetailEmpReportDb.GetDataByDate(typeId, sDate, eDate);
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailEmpReport(r));
            }
        }
        public void GetDataByDate(DataTable s)
        {
            DataTable table =s;
            Clear();
            foreach (DataRow r in table.Rows)
            {
                Add(new TradeProfitDetailEmpReport(r));
            }
        }
    }
}