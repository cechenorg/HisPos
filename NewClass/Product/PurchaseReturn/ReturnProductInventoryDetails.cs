using System.Collections.ObjectModel;

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