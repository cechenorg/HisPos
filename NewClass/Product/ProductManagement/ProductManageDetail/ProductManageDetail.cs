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

            RetailPrice = row.Field<double>("Pro_RetailPrice");
            MemberPrice = row.Field<double>("Pro_MemberPrice");
            EmployeePrice = row.Field<double>("Pro_EmployeePrice");
            SpecialPrice = row.Field<double>("Pro_SpecialPrice");
        }

        public double SingdePackagePrice { get; }
        public int SingdePackageAmount { get; }
        public double SindePrice { get; }
        public int SindeStock { get; }

        public double RetailPrice { get; }
        public double MemberPrice { get; }
        public double EmployeePrice { get; }
        public double SpecialPrice { get; }
    }
}
