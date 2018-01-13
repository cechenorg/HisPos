using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;
using System.Globalization;

namespace His_Pos
{
    class DbConnection
    {
        private string _connectionString;
       public DbConnection(string connection) { _connectionString = connection; }

        ///<summary>
        ///ChangeNameToEnglish()
        ///</summary>
        ///<remarks>
        ///輸入:SQL語法字串
        ///輸出:執行回傳資料的Datatable
        ///用途:執行SQL語法取的資料
        ///</remarks>
        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {

            var table = new DataTable();
            try
            {
                var myConn = new SqlConnection(_connectionString);
                myConn.Open();
                var myCommand = new SqlCommand(procName, myConn);
                myCommand.CommandType = CommandType.StoredProcedure;

                if (parameterList != null)
                {
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }
                }

                var sqlDapter = new SqlDataAdapter(myCommand);
                var commandBuilder = new SqlCommandBuilder(sqlDapter);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
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
                SqlConnection myConn = new SqlConnection(_connectionString);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sql, myConn);
                myCommand.Parameters.AddWithValue("@comname", Environment.MachineName);
                myCommand.Parameters.AddWithValue("@system", system);
               if (MainWindow.CurrentUser.Id == null) MainWindow.CurrentUser.Id = "";
                myCommand.Parameters.AddWithValue("@userId", MainWindow.CurrentUser.Id);
                myCommand.Parameters.AddWithValue("@functionName", functionName);
                myCommand.Parameters.AddWithValue("@systime", Convert.ToDateTime(DateTime.Now));
                myCommand.Parameters.AddWithValue("@description", description);
                myCommand.ExecuteNonQuery();
                myConn.Close();
        }//Log()



    }
}
