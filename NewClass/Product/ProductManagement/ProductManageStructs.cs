using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageStructs : ObservableCollection<ProductManageStruct>
    {
        public double TotalStockValueInRange
        {
            get { return this.Sum(p => p.StockValue); }
        }

        public ProductManageStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductManageStruct(row));
            }
        }

        internal static ProductManageStructs SearchProductByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero)
        {
            DataTable dataTable = ProductDetailDB.GetProductManageStructsByConditions(searchID, searchName, searchIsEnable, searchIsInventoryZero);

            return new ProductManageStructs(dataTable);
        }
    }
}
