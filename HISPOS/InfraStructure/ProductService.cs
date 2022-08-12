using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.NewClass.Product.ProductGroupSetting;

namespace His_Pos.InfraStructure
{
    public class ProductService
    {
        public static ProductGroupSettings GetProductGroupSettingsByID(string proID, string wareID)
        {
            var dataList = ProductGroupSettingDB.GetProductGroupSettingListByID(proID, wareID);

            ProductGroupSettings result = new ProductGroupSettings();
            foreach (var data in dataList)
            {
              result.Add(data);  
            }

            return result;

        }
    }
}
