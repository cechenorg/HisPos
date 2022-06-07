using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.WareHouse
{
    public static class WareHouseDb
    {
        public static DataTable Init()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[WareHouse]");
        }

        internal static DataTable GetWareHouseSettings()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[WareHouseSettings]");
        }

        internal static DataTable AddNewWareHouse(string newWareHouseName)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NAME", newWareHouseName));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[InsertWareHouseNewWareHouse]", parameters);
        }

        internal static DataTable DeleteWareHouse(string iD)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("WARE_ID", iD));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteWareHouseByID]", parameters);
        }
    }
}