using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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

        internal static DataTable UpdateType(int bigTypeID, string bigTypeName, int smallTypeID, string smallTypeName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("BIGTYPE_ID", bigTypeID));
            parameters.Add(new SqlParameter("BIGTYPE_NAME", bigTypeName));
            parameters.Add(new SqlParameter("SMALLTYPE_ID", smallTypeID));
            parameters.Add(new SqlParameter("SMALLTYPE_NAME", smallTypeName));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductTypeUpdateType]", parameters);
        }

        internal static DataTable DeleteType(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPE_ID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductTypeDeleteType]", parameters);
        }
    }
}