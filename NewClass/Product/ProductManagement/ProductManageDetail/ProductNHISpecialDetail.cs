using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement.ProductManageDetail
{
    internal class ProductNHISpecialDetail : ProductManageDetail
    {
        public ProductNHISpecialDetail(DataRow row) : base(row)
        {
            NHIPrice = (double)row.Field<decimal>("SpeMed_NHIPrice");
            BigCategory = row.Field<string>("SpeMed_BigCategory");
            SmallCategory = row.Field<string>("SpeMed_SmallCategory");
            Manufactory = row.Field<string>("SpeMed_Manufactory");
        }

        public double NHIPrice { get; }
        public string BigCategory { get; }
        public string SmallCategory { get; }

        public string Manufactory { get; }
    }
}