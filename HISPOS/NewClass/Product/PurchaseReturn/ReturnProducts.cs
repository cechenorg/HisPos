using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProducts : ObservableCollection<ReturnProduct>, ICloneable
    {
        public ReturnProducts()
        {
        }

        public ReturnProducts(DataTable dataTable)
        {
            ReturnProduct tempProduct = null;
            short tempID = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (tempID == 0 || tempID != row.Field<short>("StoOrdDet_ID"))
                {
                    switch (row.Field<string>("TYPE"))
                    {
                        case "O":
                            tempProduct = new ReturnOTC(row);
                            break;

                        case "M":
                            tempProduct = new ReturnMedicine(row);
                            break;
                    }

                    Add(tempProduct);
                }
                else
                {
                    tempProduct.AddInventoryDetail(row);
                }

                tempID = row.Field<short>("StoOrdDet_ID");
            }
        }

        internal static ReturnProducts GetProductsByStoreOrderID(string orederID)
        {
            return new ReturnProducts(PurchaseReturnProductDB.GetReturnProductsByStoreOrderID(orederID));
        }

        public object Clone()
        {
            ReturnProducts products = new ReturnProducts();

            foreach (var product in Items)
                products.Add(product.Clone() as ReturnProduct);

            return products;
        }

        internal void SetToProcessing()
        {
            foreach (var product in Items)
            {
                if(product.IsChecked)
                {
                    product.IsProcessing = true;
                }
            }
        }

        internal void SetStartEditToPrice()
        {
            foreach (var product in Items)
                product.StartInputVariable = ProductStartInputVariableEnum.PRICE;
        }

        internal void SetReturnInventoryDetail()
        {
            foreach (var product in this)
                product.SetReturnInventoryDetail();
        }

        internal ReturnProducts GetOldReturnProductsByStoreOrderID(string orederID)
        {
            return new ReturnProducts(PurchaseReturnProductDB.GetOldReturnProductsByStoreOrderID(orederID));
        }
    }
}