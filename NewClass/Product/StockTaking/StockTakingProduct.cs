using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.StockTaking
{
    public class StockTakingProduct : Product
    {
        #region ----- Define Variables -----
        public double Inventory { get; set; }
        public double NewInventory { get; set; }
        public double Note { get; set; }
        #endregion

        public StockTakingProduct(DataRow row) : base(row)
        {

        }

        #region ----- Define Functions -----
        #endregion
    }
}
