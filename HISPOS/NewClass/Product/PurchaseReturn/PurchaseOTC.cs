using His_Pos.Service;
using System.Data;
using DomainModel.Enum;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseOTC : PurchaseProduct
    {
        public PurchaseOTC()
        {
        }

        public PurchaseOTC(DataRow dataRow, OrderStatusEnum orderStatus) : base(dataRow, orderStatus)
        {
        }

        public override object Clone()
        {
            PurchaseOTC purchaseOtc = this.DeepCloneViaJson();

            //purchaseOtc.CloneBaseData(this);

            return purchaseOtc;
        }
    }
}