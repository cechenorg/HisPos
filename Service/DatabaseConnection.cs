using His_Pos.Class;
using His_Pos.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace His_Pos.Service
{
    public interface DatabaseConnection
    {
        void OpenConnection();
        void CloseConnection();
        void LogError(string procName, string parameters, string error);
    }
}
