using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSettings : Collection<ProductGroupSetting>
    {
        private ProductGroupSettings(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductGroupSetting(row));
            }
        }

        internal static ProductGroupSettings GetProductGroupSettingsByID(string proID, string wareID)
        {
            return new ProductGroupSettings(ProductGroupSettingDB.GetProductGroupSettingsByID(proID, wareID));
        }
    }
}