﻿using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProducts : ObservableCollection<ReturnProduct>, ICloneable
    {
        private ReturnProducts() { }
        private ReturnProducts(DataTable dataTable)
        {
            ReturnProduct tempProduct = null;

            foreach (DataRow row in dataTable.Rows)
            {
                if (tempProduct is null || !tempProduct.ID.Equals(row.Field<string>("Pro_ID")))
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
            }

            foreach (var product in this)
            {
                product.SetReturnInventoryDetail();
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
                product.IsProcessing = true;
        }
        internal void SetStartEditToPrice()
        {
            foreach (var product in Items)
                product.StartInputVariable = ProductStartInputVariableEnum.PRICE;
        }
    }
}
