﻿using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseOTC : PurchaseProduct
    {
        public PurchaseOTC() { }
        public PurchaseOTC(DataRow dataRow) : base(dataRow)
        {
        }
    }
}
