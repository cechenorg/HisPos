using System.Collections.ObjectModel;
using System.Data;
using His_Pos.FunctionWindow.AddProductWindow;

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
            DataTable dataTable = ProductDB.GetProductStructsBySearchString(searchString.Trim());
            return new ProductStructs(dataTable);
        }

        internal static int GetProductStructCountBySearchString(string searchString, AddProductEnum addProductEnum)
        {
            DataTable dataTable;
            switch (addProductEnum)
            {
                case AddProductEnum.ProductReturn:
                    dataTable = ProductDB.GetReturnProductStructCountBySearchString(searchString.Trim());
                    break;
                default:
                    dataTable = ProductDB.GetPurchaseProductStructCountBySearchString(searchString.Trim());
                    break;
            }
            return dataTable.Rows[0].Field<int>("COUNT");
        }
    }
}
