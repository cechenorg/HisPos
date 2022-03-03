using His_Pos.NewClass.StoreOrder;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProducts : ObservableCollection<PurchaseProduct>, ICloneable
    {
        public PurchaseProducts()
        {
        }

        private PurchaseProducts(DataTable dataTable)
        {
            string lastID = "";

            foreach (DataRow row in dataTable.Rows)
            {
                PurchaseProduct purchaseProduct;

                switch (row.Field<string>("TYPE"))
                {
                    case "O":
                        purchaseProduct = new PurchaseOTC(row);
                        break;

                    case "M":
                        purchaseProduct = new PurchaseMedicine(row);
                        break;

                    default:
                        purchaseProduct = null;
                        break;
                }

                if (purchaseProduct.ID.Equals(lastID))
                    purchaseProduct.IsFirstBatch = false;

                Add(purchaseProduct);

                lastID = row.Field<string>("Pro_ID");
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

        internal void SetToProcessing()
        {
            foreach (var product in Items)
                product.IsProcessing = true;
        }

        internal void SetStartEditToPrice()
        {
            //foreach (var product in Items)
                //product.StartInputVariable = ProductStartInputVariableEnum.PRICE;
        }

        public object Clone()
        {
            PurchaseProducts products = new PurchaseProducts();

            foreach (var product in Items)
                products.Add(product.Clone() as PurchaseProduct);

            return products;
        }
    }
}