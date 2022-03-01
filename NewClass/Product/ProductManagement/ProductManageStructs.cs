using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageStructs : ObservableCollection<ProductManageStruct>
    {
        public ProductManageStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductManageStruct(row));
            }
        }

        internal static ProductManageStructs SearchProductByConditions(string searchID, string searchName, bool searchIsEnable, bool searchIsInventoryZero, string wareID, bool searchIsSingdeInventory)
        {
            DataTable dataTable = ProductDetailDB.GetProductManageStructsByConditions(searchID, searchName, searchIsEnable, searchIsInventoryZero, wareID, searchIsSingdeInventory);

            return new ProductManageStructs(dataTable);
        }
    }
}