using System.Collections.ObjectModel;
using System.Data;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ReturnProducts : ObservableCollection<ReturnProduct>
    {
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
    }
}
