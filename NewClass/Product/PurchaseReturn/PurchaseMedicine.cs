using His_Pos.Service;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseMedicine : PurchaseProduct
    {
        #region ----- Define Variables -----

        public int? IsControl { get; set; }
        public bool IsFrozen { get; set; }

        #endregion ----- Define Variables -----

        public PurchaseMedicine()
        {
        }

        public PurchaseMedicine(DataRow dataRow) : base(dataRow)
        {
            IsControl = dataRow.Field<byte?>("Med_Control");
            IsFrozen = dataRow.Field<bool>("Med_IsFrozen");
        }

        public override object Clone()
        {
            PurchaseMedicine purchaseMedicine = this.DeepCloneViaJson();

            //purchaseMedicine.CloneBaseData(this);

            //purchaseMedicine.IsControl = IsControl;
            //purchaseMedicine.IsFrozen = IsFrozen;

            return purchaseMedicine;
        }
    }
}