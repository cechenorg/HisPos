using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.ProductLocation
{
    public class ProductLocationDB
    {
        internal static DataTable GetProductLocationMasters()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductLocationMaster]");
        }

        internal static DataTable GetProductLocationDetails(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductLocationDetails]", parameters);
        }

        internal static DataTable InsertProductLocationDetails(int typeID, string proid)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", typeID));
            parameters.Add(new SqlParameter("Pro_ID", proid));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductLocationDetailsInsert]", parameters);
        }

        internal static DataTable GetTypeProducts(int typeID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("TYPEID", typeID));

            return MainWindow.ServerConnection.ExecuteProc("[Get].[ProductTypeProducts]", parameters);
        }

        internal static DataTable AddNewProductLocation(string name)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NAME", name));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductLocationAddLocation]", parameters);
        }

        internal static DataTable UpdateLocation(int ID, string Name)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", ID));
            parameters.Add(new SqlParameter("NAME", Name));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductLocationEditLocation]", parameters);
        }

        internal static DataTable DeleteLocation(int ID)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", ID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[ProductLocationDeleteLocation]", parameters);
        }
    }
}