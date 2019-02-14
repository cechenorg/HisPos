using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public struct ProductManageMedicineDetail
    {
        public ProductManageMedicineDetail(DataRow row)
        {
            NHIPrice = row.Field<decimal>("Med_Price").ToString();
            SingdeMinOrderAmount = row.Field<int>("SinData_MinOrder").ToString();
            PackageAmount = row.Field<int>("SinData_PackageAmount").ToString();
            SingdePackagePrice = row.Field<decimal>("SinData_PackagePrice").ToString();
            SindePrice = row.Field<decimal>("SinData_SinglePrice").ToString();
            Form = row.Field<string>("Med_Form");
            ATC = row.Field<string>("Med_ATC");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
            ControlLevel = row.Field<byte?>("Med_Control");
            Manufactory = row.Field<string>("Med_Manufactory");
            SC = row.Field<string>("Med_SingleCompound") + "方";
            Ingredient = row.Field<string>("Med_Ingredient");
        }
        
        public bool IsFrozen { get; }
        public int? ControlLevel { get; }
        public string NHIPrice { get; }
        public string PackageAmount { get; }
        public string SingdePackagePrice { get; }
        public string SingdeMinOrderAmount { get; }
        public string SindePrice { get; }
        public string Form { get; }
        public string ATC { get; }
        public string Manufactory { get; }
        public string SC { get; }
        public string Ingredient { get; }

        public bool IsControl => !(ControlLevel is null);
    }
}
