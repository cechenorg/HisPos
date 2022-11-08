using DomainModel.Enum;
using His_Pos.ChromeTabViewModel;
using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement.ProductManageDetail
{
    public class ProductManageDetail
    {
        public ProductManageDetail(DataRow row)
        {
            SingdePackageAmount = row.Field<int>("SinData_PackageAmount");
            SingdePackagePrice = (double)row.Field<decimal>("SinData_PackagePrice");
            SindePrice = (double)row.Field<decimal>("SinData_SinglePrice");
            SindeStock = row.Field<string>("SinData_Stock");

            if (row.Table.Columns.Contains("Pro_RetailPrice"))
                RetailPrice = row.Field<double>("Pro_RetailPrice");
            if (row.Table.Columns.Contains("Pro_MemberPrice"))
                MemberPrice = row.Field<double>("Pro_MemberPrice");
            if (row.Table.Columns.Contains("Pro_EmployeePrice"))
                EmployeePrice = row.Field<double>("Pro_EmployeePrice");
            if (row.Table.Columns.Contains("Pro_SpecialPrice"))
                SpecialPrice = row.Field<double>("Pro_SpecialPrice");
            
            if(row.Table.Columns.Contains("Pro_IsCommon"))
                IsCommon = row.Field<bool>("Pro_IsCommon");
            if (row.Table.Columns.Contains("Pro_IsDeposit"))
                IsDeposit = row.Field<bool>("Pro_IsDeposit");

            IsControl = !(!IsDeposit || (IsDeposit && ViewModelMainWindow.CurrentUser.Authority == Authority.Admin));
        }

        public double SingdePackagePrice { get; }
        public int SingdePackageAmount { get; }
        public double SindePrice { get; }
        public string SindeStock { get; }

        public double RetailPrice { get; set; }
        public double MemberPrice { get; set; }
        public double EmployeePrice { get; set; }
        public double SpecialPrice { get; set; }
        public bool IsCommon { get; set; }
        public bool IsDeposit { get; set; }

        public bool IsControl { get; set; }
    }
}