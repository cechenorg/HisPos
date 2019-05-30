using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.StockTaking;

namespace His_Pos.NewClass.StockTaking.StockTakingPlan
{
    public class StockTakingPlan
    {
        #region ----- Define Variables -----
        public string ID { get; set; }
        public string Name { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public StockTakingProducts StockTakingProductCollection { get; set; }
        #endregion

        public StockTakingPlan(DataRow row)
        {

        }

        #region ----- Define Functions -----
        #endregion
    }
}
