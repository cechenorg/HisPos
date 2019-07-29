using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Trade.TradeRecord
{
    public class TradeRecordMasters : Collection<TradeRecordMaster>
    {
        #region ----- Define Variables -----
        public int TotalPrice { get; set; }
        public int TotalPriceIn90Days { get; set; }
        #endregion

        #region ----- Define Functions -----
        #endregion
    }
}
