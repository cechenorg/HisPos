using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeRecordDetails : Collection<TradeRecordDetail>
    {
        public TradeRecordDetails(DataTable table)
        {
            int i = 1;
            foreach (DataRow dr in table.Rows)
            {
                TradeRecordDetail trade = new TradeRecordDetail(dr);
                trade.RowNo = Count + 1;
                Add(trade);
            }
        }
    }
}