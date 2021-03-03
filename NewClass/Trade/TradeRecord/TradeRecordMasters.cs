using System.Collections.ObjectModel;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeRecordMasters : Collection<TradeRecordMaster>
    {
        #region ----- Define Variables -----

        public int TotalPrice { get; set; }
        public int TotalPriceIn90Days { get; set; }

        #endregion ----- Define Variables -----
    }
}