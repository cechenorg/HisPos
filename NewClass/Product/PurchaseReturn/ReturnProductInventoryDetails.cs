using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProductInventoryDetails : Collection<ReturnProductInventoryDetail>
    {
        internal void ClearReturnValue()
        {
            foreach (var inventoryDetail in this)
            {
                inventoryDetail.ReturnAmount = 0;
            }
        }
    }
}
