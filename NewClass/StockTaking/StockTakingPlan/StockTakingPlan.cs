using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace His_Pos.NewClass.StockTaking.StockTakingPlan
{
    public class StockTakingPlan
    {
        #region ----- Define Variables -----
        public int ID { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public  StockTakingPlanProducts StockTakingProductCollection { get; set; }
        #endregion
        public StockTakingPlan() {
            StockTakingProductCollection = new StockTakingPlanProducts();
        }
        public StockTakingPlan(DataRow row)
        {
            ID = row.Field<int>("StoTakPlanMas_ID");
            Name = row.Field<string>("StoTakPlanMas_Name");
            WareHouse = new WareHouse.WareHouse(row);
            Note = row.Field<string>("StoTakPlanMas_Note");
            StockTakingProductCollection = new StockTakingPlanProducts();
        }

        #region ----- Define Functions -----
        internal void NewStockTakingPlan() {
            StockTakingDB.NewStockTakingPlan(this);
        }
        internal void Delete()
        {
            StockTakingDB.DeleteStockTakingPlan(this); 
        }
        internal void Update() {
            StockTakingDB.UpdateStockTakingPlan(this);
        }
        internal void GetPlanProducts()
        {
            StockTakingProductCollection.Clear();
            StockTakingPlanProducts temp = new StockTakingPlanProducts(StockTakingDB.GetStockTakingPlanProductsByID(ID));
            foreach (var s in temp) {
                StockTakingProductCollection.Add(s);
            }
        }
        #endregion
    }
}
