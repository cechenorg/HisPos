using System.Collections.ObjectModel;
using System.Data;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProducts : ObservableCollection<PurchaseProduct>
    {
        private PurchaseProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                switch (row.Field<string>("TYPE"))
                {
                    case "O":
                        Add(new PurchaseOTC(row));
                        break;
                    case "M":
                        Add(new PurchaseMedicine(row));
                        break;
                }
            }
        }
        
        internal static PurchaseProducts GetProductsByStoreOrderID(string orederID)
        {
            return new PurchaseProducts(PurchaseReturnProductDB.GetProductsByStoreOrderID(orederID));
        }

        internal static void UpdateSingdeProductsByStoreOrderID(string orederID)
        {
            DataTable dataTable = PurchaseReturnProductDB.GetSingdeProductsByStoreOrderID(orederID);

            StoreOrderDB.UpdateSingdeProductsByStoreOrderID(dataTable, orederID);
        }
    }
}
