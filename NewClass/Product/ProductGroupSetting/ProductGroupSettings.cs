using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSettings :  Collection<ProductGroupSetting>
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
