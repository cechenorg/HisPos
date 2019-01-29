using His_Pos.NewClass.Product.Medicine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class PurchaseProducts : ObservableCollection<PurchaseProduct>
    {
        private PurchaseProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                switch (row.Field<string>("Usa_PrintName"))
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
    }
}
