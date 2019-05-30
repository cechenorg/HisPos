using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.StockTaking
{
    public class StockTakingProducts : Collection<StockTakingProduct>
    {
        private StockTakingProducts(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new StockTakingProduct(row));
            }
        }
    }
}
