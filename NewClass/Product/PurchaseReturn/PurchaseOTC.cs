using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseOTC : PurchaseProduct
    {
        public PurchaseOTC() { }
        public PurchaseOTC(DataRow dataRow) : base(dataRow)
        {
        }

        public override object Clone()
        {
            PurchaseOTC purchaseOtc = new PurchaseOTC();

            purchaseOtc.CloneBaseData(this);

            return purchaseOtc;
        }
    }
}
