using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product.ProductManagement
{
    public class ProductInventoryRecords : Collection<ProductInventoryRecord>
    {
        public ProductInventoryRecords(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductInventoryRecord(row));
            }
        }

        internal static ProductInventoryRecords GetInventoryRecordsByID(string id)
        {
            return new ProductInventoryRecords(ProductDetailDB.GetInventoryRecordsByID(id));
        }
    }
}
