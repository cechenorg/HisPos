using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouseSetting : WareHouse
    {
        #region ----- Define Variables -----
        public double StockValue { get; set; }
        public double NegativeStockValue { get; set; }
        public double TotalStockValue { get { return StockValue + NegativeStockValue; } }
        #endregion

        public WareHouseSetting(DataRow row) : base(row)
        {
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
            if (StockValue > 0 || NegativeStockValue < 0)
            {
                MessageWindow.ShowMessage("欲刪除之庫別庫存現值及負庫需清空!", MessageType.ERROR);
                return false;
            }

            return true;
        }
    }
}
