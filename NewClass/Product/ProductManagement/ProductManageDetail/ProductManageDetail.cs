using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement.ProductManageDetail
{
    public class ProductManageDetail
    {
        public ProductManageDetail(DataRow row)
        {
            SingdeMinOrderAmount = row.Field<int>("SinData_MinOrder").ToString();
            SingdePackagePrice = (double)row.Field<decimal>("SinData_PackagePrice");
            SindePrice = (double)row.Field<decimal>("SinData_SinglePrice");
            SindeStock = row.Field<int>("SinData_Stock");
        }

        public double SingdePackagePrice { get; }
        public string SingdeMinOrderAmount { get; }
        public double SindePrice { get; }
        public int SindeStock { get; }
    }
}
