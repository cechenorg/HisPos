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
        public PurchaseMedicine(DataRow dataRow) : base(dataRow)
        {
        }
    }
}
