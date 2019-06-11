using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeManageMasters : Collection<ProductTypeManageMaster>
    {
        private ProductTypeManageMasters(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductTypeManageMaster(row));
            }
        }

        internal static ProductTypeManageMasters GetProductTypeMasters()
        {
            return new ProductTypeManageMasters(ProductTypeDB.GetProductTypeMasters());
        }
    }
}
