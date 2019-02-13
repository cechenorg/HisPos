using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            DataTable dataTable = ProductDB.GetProductStructsBySearchString(searchString);
            return new ProductStructs(dataTable);
        }

        internal static int GetProductStructCountBySearchString(string searchString, AddProductEnum addProductEnum)
        {
            DataTable dataTable;

            switch (addProductEnum)
            {
                case AddProductEnum.ProductPurchase:
                    dataTable = ProductDB.GetPurchaseProductStructCountBySearchString(searchString);
                    break;
                case AddProductEnum.ProductReturn:
                    dataTable = ProductDB.GetReturnProductStructCountBySearchString(searchString);
                    break;
                default:
                    dataTable = new DataTable();
                    break;
            }

            return dataTable.Rows[0].Field<int>("COUNT");
        }
    }
}
