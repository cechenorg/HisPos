using System.Collections.ObjectModel;
using System.Data;

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