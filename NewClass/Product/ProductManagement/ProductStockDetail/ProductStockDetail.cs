using GalaSoft.MvvmLight;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement.ProductStockDetail
{
    public class ProductStockDetail : ObservableObject
    {
        #region ----- Define Variables -----

        public double OnTheWayAmount { get; set; }
        public double TotalInventory { get; set; }
        public double StockValue { get; set; }
        public double LastPrice { get; set; }

        #endregion ----- Define Variables -----

        public ProductStockDetail(DataRow row)
        {
            StockValue = row.Field<double>("STOCK_VALUE");
            TotalInventory = row.Field<double>("Inv_Inventory");
            OnTheWayAmount = row.Field<double>("Inv_OnTheWay");
            LastPrice = (double)row.Field<decimal>("Inv_LastPrice");
        }
    }
}