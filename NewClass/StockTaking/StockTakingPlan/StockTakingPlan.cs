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
        public int ID { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public StockTakingProducts StockTakingProductCollection { get; set; }
        #endregion

        public StockTakingPlan(DataRow row)
        {
            ID = row.Field<int>("StoTakPlanMas_ID");
            Name = row.Field<string>("StoTakPlanMas_Name");
            WareHouse = new WareHouse.WareHouse(row);
            Note = row.Field<string>("StoTakPlanMas_Note");
        }
        
        #region ----- Define Functions -----
        internal void Delete()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
