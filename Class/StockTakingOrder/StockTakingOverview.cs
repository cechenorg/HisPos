using His_Pos.Interface;
using System.Data;

namespace His_Pos.Class.StockTakingOrder
{
    public class StockTakingOverview : IStockTakingRecord
    {
        public StockTakingOverview(DataRow dataRow)
        {
            StockTakingId = dataRow["PROCHE_ID"].ToString();
            StockTakingDate = dataRow["PROCHE_DATE"].ToString();
            EmpName = dataRow["EMP_NAME"].ToString();
            OldValue = dataRow["PROCHE_OLDVAL"].ToString();
            NewValue = dataRow["PROCHE_NEWVAL"].ToString();
        }

        //PROCHE_ID, EMP_NAME, PROCHE_DATE, PROCHE_OLDVAL, PROCHE_NEWVAL
        public string StockTakingId { get; set; }

        public string StockTakingDate { get; set; }
        public string EmpName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}