using System;
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

        internal static bool UpdateSingdeProductsByStoreOrderID(string orederID, string receiveID)
        {
            DataTable dataTable = PurchaseReturnProductDB.GetSingdeProductsByStoreOrderID(orederID);

            if (dataTable.Rows.Count == 0) return false;

            dataTable = StoreOrderDB.UpdateSingdeProductsByStoreOrderID(dataTable, orederID, receiveID);

            if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                return false;

            return true;
        }

        internal void SetToSingde()
        {
            foreach (var product in Items)
                product.IsSingde = true;
        }
    }
}
