using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class  ChooseBatchProduct
    {
        public ChooseBatchProduct(DataRow row)
        {
            BatchNumber = row.Field<string>("InvDet_BatchNumber");
            BatchNumberLimit = (int)row.Field<double>("BATCH_LIMIT");
            ReturnAmount = row.Field<int>("RETURN_AMOUNT");
        }

        public string BatchNumber { get; }
        public int BatchNumberLimit { get; }
        public int ReturnAmount { get; set; }
    }
}
