using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnMedicine : ReturnProduct
    {
        #region ----- Define Variables -----
        public bool IsCommon { get; set; }
        public int? IsControl { get; set; }
        public bool IsFrozen { get; set; }
        #endregion

        public ReturnMedicine(DataRow row) : base(row)
        {
            IsCommon = row.Field<bool>("Med_IsCommon");
            IsControl = row.Field<byte?>("Med_Control");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
        }
    }
}
