using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Product
{
    public class ProductStructs : Collection<ProductStruct>
    {
        public ProductStructs(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductStruct(row));
            }
        }
        internal static ProductStructs GetProductStructsBySearchString(string searchString)
        {
            DataTable dataTable = ProductDB.GetProductStructsBySearchString(searchString);
            return new ProductStructs(dataTable);
        }
    }
}
