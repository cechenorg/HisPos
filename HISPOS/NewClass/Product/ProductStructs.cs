using His_Pos.FunctionWindow.AddProductWindow;
using System.Collections.ObjectModel;
using System.Data;

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

        internal static ProductStructs GetProductStructsBySearchString(string searchString, string wareID)
        {
            DataTable dataTable = ProductDB.GetProductStructsBySearchString(searchString.Trim(), wareID);
            return new ProductStructs(dataTable);
        }

        internal static int GetProductStructCountBySearchString(string searchString, AddProductEnum addProductEnum,string wareID = "0")
        {
            DataTable dataTable;
            switch (addProductEnum)
            {
                case AddProductEnum.ProductReturn:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 1);
                    break;

                case AddProductEnum.Trade:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 2);
                    break;

                default:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 0);
                    break;
            }
            return dataTable.Rows[0].Field<int>("COUNT");
        }
    }
}