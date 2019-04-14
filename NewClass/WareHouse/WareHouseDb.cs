using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.WareHouse
{
    public static class WareHouseDb
    {
        public static DataTable Init() { 
            return MainWindow.ServerConnection.ExecuteProc("[Get].[WareHouse]");
        }
    }
}
