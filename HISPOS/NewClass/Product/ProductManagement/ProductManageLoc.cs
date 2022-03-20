using System.Data;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductManageLoc
    {
        public ProductManageLoc(DataRow row)
        {
          
            ID = row.Field<string>("Pro_ID");
            Pro_ChineseName = row.Field<string>("Pro_ChineseName");
            Inv_Inventory = row.Field<decimal>("Inv_Inventory");
            DepRec_Amount = row.Field<int>("DepRec_Amount");
            aaaa = row.Field<string>("aaaa");
            DepRec_Amount = row.Field<int>("DepRec_Amount");
            Total = row.Field<int>("Total");
        }

        public string ID { get; set; }
        public string Pro_ChineseName { get; set; }
        public decimal Inv_Inventory { get; set; }
        public int DepRec_Amount { get; set; }
        public string aaaa { get; set; }
        public int AvgPrice { get; set; }
        public int Total { get; set; }

    }
}