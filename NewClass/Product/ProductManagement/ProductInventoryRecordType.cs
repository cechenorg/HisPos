using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public enum ProductInventoryRecordType
    {
        Error = 0,
        PurchaseReturn = 1,
        StockTaking = 2,
        Prescription = 3,
        Transaction = 4,
        All = 5,
        MergeSplit = 6
    }
}
