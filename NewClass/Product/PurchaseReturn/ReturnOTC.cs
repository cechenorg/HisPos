using His_Pos.Service;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnOTC : ReturnProduct
    {
        public ReturnOTC()
        {
        }

        public ReturnOTC(DataRow row) : base(row)
        {
        }

        public override object Clone()
        {
            ReturnOTC returnOtc = this.DeepCloneViaJson();

            return returnOtc;
        }
    }
}