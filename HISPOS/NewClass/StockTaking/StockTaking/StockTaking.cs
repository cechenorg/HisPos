using GalaSoft.MvvmLight;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using System;
using System.Data;

namespace His_Pos.NewClass.StockTaking.StockTaking
{
    public class StockTaking : ObservableObject
    {
        public StockTaking()
        {
            StockTakingProductCollection = new StockTakingProducts();
        }

        public StockTaking(DataRow r)
        {
            ID = r.Field<string>("StoTakMas_ID");
            WareHouse = ChromeTabViewModel.ViewModelMainWindow.GetWareHouse(r.Field<int>("StoTakMas_WarehouseID").ToString());
            Employee = EmployeeService.GetDataByID(r.Field<int>("Emp_ID"));
            Time = r.Field<DateTime>("StoTakMas_Time");
            TotalValueDiff = r.Field<double>("TotalValueDiff");
            StockTakingProductCollection = new StockTakingProducts();
        }

        public string ID { get; set; }
        public double TotalValueDiff { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public Employee Employee { get; set; } = new Employee();
        public DateTime Time { get; set; }
        public StockTakingProduct.StockTakingProducts StockTakingProductCollection { get; set; }

        public void GetStockTakingProductbyID()
        {
            StockTakingProductCollection = StockTakingProductCollection.GetStockTakingProductsByID(ID);
        }

        public void InsertStockTaking(string typeName)
        {
            StockTakingDB.InsertStockTaking(this, typeName);
        }

        public void SingleStockTaking(string id, double inventory, double newInventory, double takingPrice, WareHouse.WareHouse wareHouse)
        {
            WareHouse = wareHouse;
            StockTakingProduct.StockTakingProduct stockTakingProduct = new StockTakingProduct.StockTakingProduct();
            stockTakingProduct.ID = id;
            stockTakingProduct.Inventory = inventory;
            stockTakingProduct.NewInventory = newInventory;
            stockTakingProduct.TakingPrice = takingPrice;//盤盈品項金額
            StockTakingProductCollection.Add(stockTakingProduct);
            InsertStockTaking("單品盤點");
        }

        public void InsertStockChange(string typeName, string Number)
        {
            StockTakingDB.InsertStockChange(this, typeName, Number);
        }
        public void InsertTransfer(int type, int manID, string proID, string batchNum, decimal qty, decimal price, int warID)
        {
            StockTakingDB.InsertTransfer(type, manID, proID, batchNum, qty, price, warID);
        }

        public void SingleStockChange(string id, double inventory, double newInventory, double takingPrice, WareHouse.WareHouse wareHouse, string Number)
        {
            WareHouse = wareHouse;
            StockTakingProduct.StockTakingProduct stockTakingProduct = new StockTakingProduct.StockTakingProduct();
            stockTakingProduct.ID = id;
            stockTakingProduct.Inventory = inventory;
            stockTakingProduct.NewInventory = newInventory;
            stockTakingProduct.TakingPrice = takingPrice;//盤盈品項金額
            StockTakingProductCollection.Add(stockTakingProduct);
            InsertStockChange("轉受讓", Number);
        }
    }
}