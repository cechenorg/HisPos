using System;
using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProducts : ObservableCollection<ReturnProduct>, ICloneable
    {
        private ReturnProducts() { }
        private ReturnProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                switch (row.Field<string>("TYPE"))
                {
                    case "O":
                        Add(new ReturnOTC(row));
                        break;
                    case "M":
                        Add(new ReturnMedicine(row));
                        break;
                }
            }
        }

        internal static ReturnProducts GetProductsByStoreOrderID(string orederID)
        {
            return new ReturnProducts(PurchaseReturnProductDB.GetProductsByStoreOrderID(orederID));
        }

        public object Clone()
        {
            ReturnProducts products = new ReturnProducts();

            foreach (var product in Items)
                products.Add(product.Clone() as ReturnProduct);

            return products;
        }
    }
}
