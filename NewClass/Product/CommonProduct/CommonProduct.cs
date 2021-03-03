using System.Data;

namespace His_Pos.NewClass.Product.CommonProduct
{
    public class CommonProduct : Product
    {
        public CommonProduct(DataRow r) : base(r)
        {
            OnTheFramAmount = r.Field<double>("OnTheFrame");
            BasicAmount = r.Field<double>("BasicAmount");
            PurchaseAmount = r.Field<double>("PurchaseAmount");
            TypeID = (r.Field<int>("TypeID"));
        }

        public double OnTheFramAmount { get; set; }
        public double BasicAmount { get; set; }
        public double PurchaseAmount { get; set; }
        public int TypeID { get; set; }
    }
}