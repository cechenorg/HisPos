using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement.ProductManageDetail
{
    public class ProductNHIDetail : ProductManageDetail
    {
        public ProductNHIDetail(DataRow row) : base(row)
        {
            NHIPrice = (double)row.Field<decimal>("Med_Price");
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
        public double NHIPrice { get; }

        public string Form { get; }
        public string ATC { get; }
        public string Manufactory { get; }
        public string SC { get; }
        public string Ingredient { get; }

        public bool IsControl => !(ControlLevel is null);
    }
}