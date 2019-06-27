using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Castle.MicroKernel;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageStructs : ObservableCollection<ProductManageStruct>
    {
        public double TotalStockValueInRange { get; set; } = 0;

        public ProductManageStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductManageStruct(row));
            }

            if(dataTable.Rows.Count > 0)
                TotalStockValueInRange = (double)dataTable.Rows[0].Field<decimal>("SELECT_RANGE_STOCK");
        }

        internal static ProductManageStructs SearchProductByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero, string wareID)
        {
            DataTable dataTable = ProductDetailDB.GetProductManageStructsByConditions(searchID, searchName, searchIsEnable, searchIsInventoryZero, wareID);

            return new ProductManageStructs(dataTable);
        }
    }
}
