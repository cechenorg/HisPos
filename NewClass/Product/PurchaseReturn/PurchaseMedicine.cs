using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseMedicine : PurchaseProduct
    {
        #region ----- Define Variables -----
        public bool IsCommon { get; set; }
        public int? IsControl { get; set; }
        public bool IsFrozen { get; set; }
        #endregion

        public PurchaseMedicine(DataRow dataRow) : base(dataRow)
        {
            IsCommon = dataRow.Field<bool>("Med_IsCommon");
            IsControl = dataRow.Field<byte?>("Med_Control");
            IsFrozen = dataRow.Field<bool>("Med_IsFrozen");
        }
    }
}
