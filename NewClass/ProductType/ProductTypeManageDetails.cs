using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeManageDetails : Collection<ProductTypeManageDetail>
    {
        private ProductTypeManageDetails(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new ProductTypeManageDetail(row));
            }

        }
        internal static ProductTypeManageDetails GetProductTypeDetails(int iD)
        {
            return new ProductTypeManageDetails(ProductTypeDB.GetProductTypeDetails(iD));
        }
    }
}
