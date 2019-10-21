using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductGroupSetting
{
    public class ProductGroupSetting : Product
    {
        public string WareHouseID { get; set; }

        public ProductGroupSetting(DataRow row) : base(row)
        {
            WareHouseID = row.Field<int>("WARE_ID").ToString();
        }
    }
}
