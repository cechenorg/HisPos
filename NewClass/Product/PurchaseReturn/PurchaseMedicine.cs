using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            IsControl = dataRow.Field<int?>("Med_Control");
            IsFrozen = dataRow.Field<bool>("Med_IsFrozen");
        }
    }
}
