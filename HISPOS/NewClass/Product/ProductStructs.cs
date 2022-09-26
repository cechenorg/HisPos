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

        internal static ProductStructs GetProductStructsBySearchString(string searchString, string wareID, int noOTC)
        {
            DataTable dataTable = ProductDB.GetProductStructsBySearchString(searchString.Trim(), wareID, noOTC);
            return new ProductStructs(dataTable);
        }

        internal static int GetProductStructCountBySearchString(string searchString, AddProductEnum addProductEnum,string wareID = "0")
        {
            DataTable dataTable;
            int noOTC = (addProductEnum == AddProductEnum.PrescriptionDeclare || addProductEnum == AddProductEnum.PrescriptionEdit) ? 1 : 0;
            switch (addProductEnum)
            {
                case AddProductEnum.ProductReturn:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 1, noOTC);
                    break;

                case AddProductEnum.Trade:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 2, noOTC);
                    break;

                default:
                    dataTable = ProductDB.GetProductStructCountBySearchString(searchString.Trim(), wareID, 0, noOTC);
                    break;
            }
            return dataTable.Rows[0].Field<int>("COUNT");
        }
    }
}