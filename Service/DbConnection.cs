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

        public DbConnection()
        {
        }

        public DbConnection(string connection) { _connection = new SqlConnection(connection); }

        public void NonQueryBySqlString(string sqlString)
        {
            DataTable dataTable = new DataTable();
            SqlCommand cmd = new SqlCommand(sqlString, _connection);
            try
            {
                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

        }
        public DataTable QueryBySqlString(string sqlString)
        {
            DataTable dataTable = new DataTable();
            SqlCommand cmd = new SqlCommand(sqlString, _connection);
            try {
                _connection.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
                _connection.Close();
                da.Dispose();
                return dataTable;
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
        public void Log(string system, string functionName, string description)
        {
            string sql = @"Insert into TestDB.dbo.WebLog (COMNAME,SYSTEM,USER_ID,FUNCTION_NAME,SYSTIME,DESCRIPTION) 
                                                   Values(@comname,@system,@userId,@functionName,@systime,@description)";

                _connection.Open();
                SqlCommand myCommand = new SqlCommand(sql, _connection);
                myCommand.Parameters.AddWithValue("@comname", Environment.MachineName);
                myCommand.Parameters.AddWithValue("@system", system);
               if (MainWindow.CurrentUser.Id == null) MainWindow.CurrentUser.Id = "";
                myCommand.Parameters.AddWithValue("@userId", MainWindow.CurrentUser.Id);
                myCommand.Parameters.AddWithValue("@functionName", functionName);
                myCommand.Parameters.AddWithValue("@systime", Convert.ToDateTime(DateTime.Now));
                myCommand.Parameters.AddWithValue("@description", description);
                myCommand.ExecuteNonQuery();
                _connection.Close();
        }//Log()

        public DataTable SetProcName(string procName,DbConnection connection)
        {
            var table = connection.ExecuteProc(procName);
            return table;
        }
    }
}
