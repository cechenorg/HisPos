using His_Pos.Class;
using His_Pos.FunctionWindow;
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

        internal bool DeleteWareHouse()
        {
            DataTable dataTable = WareHouseDb.DeleteWareHouse(ID);

            if (dataTable is null || dataTable.Rows.Count == 0)
                return false;
            else
                return dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS");
        }

        internal bool IsDeletable()
        {
            if (!CanDelete)
            {
                MessageWindow.ShowMessage("欲刪除之庫別庫存需清空!", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}