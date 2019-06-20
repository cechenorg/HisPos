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
        public string Note { get; set; }
        #endregion

        public StockTakingProduct(DataRow row) : base(row)
        {
            Inventory = row.Field<double>("StoTakDet_OldValue");
            NewInventory = row.Field<double>("StoTakDet_NewValue");
            Note = row.Field<string>("StoTakDet_Note");
        }

        #region ----- Define Functions -----
        #endregion
    }
}
