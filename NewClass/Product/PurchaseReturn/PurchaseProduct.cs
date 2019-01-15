using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProduct : Product
    {
        public PurchaseProduct() : base()
        {
        }
        public PurchaseProduct(DataRow dataRow) : base(dataRow)
        {
        }
    }
}
