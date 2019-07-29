using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTaking
{
    public class StockTaking : ObservableObject
    {
        public StockTaking() {
            StockTakingProductCollection = new StockTakingProducts();
        }
        public StockTaking(DataRow r) {
            ID = r.Field<string>("StoTakMas_ID");
            WareHouse = ChromeTabViewModel.ViewModelMainWindow.GetWareHouse(r.Field<int>("StoTakMas_WarehouseID").ToString());
            Employee =  ChromeTabViewModel.ViewModelMainWindow.GetEmployeeByID(r.Field<int>("Emp_ID"));
            Time = r.Field<DateTime>("StoTakMas_Time");
            StockTakingProductCollection = new StockTakingProducts();
        }
        public string ID { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public Employee Employee { get; set; }
        public DateTime Time { get; set; }
        public StockTakingProduct.StockTakingProducts StockTakingProductCollection { get; set; }
        public void GetStockTakingProductbyID() {
            StockTakingProductCollection = StockTakingProductCollection.GetStockTakingProductsByID(ID); 
        }
        public void InsertStockTaking(string typeName)
        {
            StockTakingDB.InsertStockTaking(this, typeName);
        }
       
    }
}
