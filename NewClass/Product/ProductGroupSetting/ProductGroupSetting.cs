using System.Data;

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