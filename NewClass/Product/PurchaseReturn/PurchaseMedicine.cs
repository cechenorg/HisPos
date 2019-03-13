using System.Data;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseMedicine : PurchaseProduct
    {
        #region ----- Define Variables -----
        public bool IsCommon { get; set; }
        public int? IsControl { get; set; }
        public bool IsFrozen { get; set; }
        #endregion

        public PurchaseMedicine() { }

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
