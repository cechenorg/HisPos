using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ProductType
{
    public class ProductTypeDB
    {
        internal static DataTable GetProductTypeMasters()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTypeMaster]");
        }

        internal static DataTable GetProductTypeDetails(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPEID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTypeDetails]", parameters);
        }

        internal static DataTable GetTypeProducts(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPEID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTypeProducts]", parameters);
        }

        internal static DataTable AddNewProductType(string name, int parentID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NAME", name));
            parameters.Add(new SqlParameter("PARENT", parentID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductTypeAddType]", parameters);
        }

        internal static DataTable DeleteType(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE_ID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductTypeDeleteType]", parameters);
        }
    }
}
