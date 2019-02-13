using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.PurchaseReturn
{
    public class ChooseBatchProducts : Collection<ChooseBatchProduct>
    {
        public ChooseBatchProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ChooseBatchProduct(row));
            }
        }

        internal static ChooseBatchProducts GetChooseBatchProductsByID(string iD)
        {
            DataTable dataTable = PurchaseReturnProductDB.GetChooseBatchProductsByID(iD);

            return new ChooseBatchProducts(dataTable);
        }
    }
}
