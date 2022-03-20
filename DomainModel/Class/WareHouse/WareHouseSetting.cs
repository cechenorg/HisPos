using His_Pos.NewClass;
using System.Data;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouseSetting : WareHouse
    {
        #region ----- Define Variables -----

        public bool CanDelete { get; set; }
        public double StockValue { get; set; }
        public double NegativeStockValue { get; set; }
        public double TotalStockValue { get { return StockValue + NegativeStockValue; } }

        #endregion ----- Define Variables -----

        public WareHouseSetting(DataRow row) : base(row)
        {
            CanDelete = row.Field<bool>("CAN_DELETE");
            StockValue = row.Field<double>("STOCKVALUE");
            NegativeStockValue = row.Field<double>("NEG_STOCKVALUE");
        }
         
    }
}