﻿using System.Data;
using His_Pos.Service;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnMedicine : ReturnProduct
    {
        #region ----- Define Variables -----
        public bool IsCommon { get; set; }
        public int? IsControl { get; set; }
        public bool IsFrozen { get; set; }
        #endregion

        public ReturnMedicine() { }
        public ReturnMedicine(DataRow row) : base(row)
        {
            IsControl = row.Field<byte?>("Med_Control");
            IsFrozen = row.Field<bool>("Med_IsFrozen");
        }

        public override object Clone()
        {
            ReturnMedicine returnMedicine = this.DeepCloneViaJson();

            return returnMedicine;
        }
    }
}
