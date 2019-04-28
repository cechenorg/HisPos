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
            SingdePackageAmount = row.Field<int>("SinData_PackageAmount");
            SingdePackagePrice = (double)row.Field<decimal>("SinData_PackagePrice");
            SindePrice = (double)row.Field<decimal>("SinData_SinglePrice");
            SindeStock = row.Field<int>("SinData_Stock");
        }

        public double SingdePackagePrice { get; }
        public int SingdePackageAmount { get; }
        public double SindePrice { get; }
        public int SindeStock { get; }
    }
}
