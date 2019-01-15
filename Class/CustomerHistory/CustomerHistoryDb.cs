using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.CustomerHistory
{
    public static class CustomerHistoryDb
    {
        public static DataTable GetData(int id)
        {
            var table = new DataTable();
            return table;
        }
    }
}
