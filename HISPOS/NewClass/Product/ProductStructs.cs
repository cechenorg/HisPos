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
                    dataTable = ProductDB.GetReturnProductStructCountBySearchString(searchString.Trim(), wareID);
                    break;

                case AddProductEnum.Trade:
                    dataTable = ProductDB.GetTradeProductStructCountBySearchString(searchString.Trim());
                    break;

                default:
                    dataTable = ProductDB.GetPurchaseProductStructCountBySearchString(searchString.Trim(), wareID);
                    break;
            }
            return dataTable.Rows[0].Field<int>("COUNT");
        }
    }
}