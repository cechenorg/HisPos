using System;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeRecordMaster
    {
        #region ----- Define Variables -----

        public DateTime Date { get; set; }
        public int TotalPrice { get; set; }
        public TradeRecordDetails Details { get; set; }

        #endregion ----- Define Variables -----
    }
}