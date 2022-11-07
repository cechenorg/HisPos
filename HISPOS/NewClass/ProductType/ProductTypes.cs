using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypes : ObservableCollection<ProductType>
    {
        public ProductTypes(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductType(row));
            }
        }
    }
}
