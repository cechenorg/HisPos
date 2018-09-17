using His_Pos.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace His_Pos.Service
{
    class DbConnection
    {
        private SqlConnection _connection;
        private string _connectionString;
        public DbConnection()
        {
        }

        public DbConnection(string connection) { _connection = new SqlConnection(connection); _connectionString = connection; }

        public void MySqlNonQueryBySqlString(string sqlString)
        { 
            MySqlConnection conn = new MySqlConnection(_connectionString);
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex) {
                throw new InvalidOperationException(ex.Message);
            }
        }
       
        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {
            var table = new DataTable();
            try
            {
                _connection.Open();
                var myCommand = new SqlCommand(procName, _connection);
                myCommand.CommandType = CommandType.StoredProcedure;

                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
                _connection.Close();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return table;
        }

        ///<summary>
        ///ChangeNameToEnglish()
        ///</summary>
        ///<remarks>
        ///輸入:所在系統名 函式名稱 敘述
        ///輸出:
        ///用途:紀錄log
        ///</remarks>
        public void Log(string procName, List<SqlParameter> parameterList)
        {
            string parameValues = string.Empty;
            foreach (SqlParameter row in parameterList)
            {
                parameValues += row.ParameterName + ":" + row.Value.ToString() + "\r\n";
            }
            var dd = new DbConnection(Settings.Default.SQL_global);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PROC_NAME", procName));
            parameters.Add(new SqlParameter("PROC_PARAM", parameValues));
            var table = dd.ExecuteProc("[HIS_POS_DB].[LOG].[SETPROCLOG]", parameters);
        }//Log()

     
    }
}
