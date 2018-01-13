using System;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.Service
{
    class Globalservice
    {
        public string ConnectionString = Properties.Settings.Default.SQL_global;

        public DataTable GetData(string sql)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sql, ConnectionString);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);
            var table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            return table;
        }//GetData()

        public bool NonQuery(string sql)
        {
            bool status = false;
            try
            {
                SqlConnection myConn = new SqlConnection(ConnectionString);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sql, myConn);
                myCommand.ExecuteNonQuery();
                myConn.Close();
                status = true;
                Log("globalservice", "NonQuery", "Success:" + sql);
            }
            catch (Exception ex)
            {
                Log("globalservice", "NonQuery", "Fail:" + sql);
            }
            return status;
        }
        public void Log(string system, string functionName, string description)
        {
            string sql = @"Insert into TestDB.dbo.WebLog (COMNAME,SYSTEM,USER_ID,FUNCTION_NAME,SYSTIME,DESCRIPTION) 
                                                   Values(@comname,@system,@userId,@functionName,@systime,@description)";
            try
            {
                SqlConnection myConn = new SqlConnection(ConnectionString);
                myConn.Open();
                SqlCommand myCommand = new SqlCommand(sql, myConn);
                myCommand.Parameters.AddWithValue("@comname", Environment.MachineName);
                myCommand.Parameters.AddWithValue("@system", system);
                myCommand.Parameters.AddWithValue("@userId",MainWindow.CurrentUser.UserId);
                myCommand.Parameters.AddWithValue("@functionName", functionName);
                myCommand.Parameters.AddWithValue("@systime", Convert.ToDateTime(DateTime.Now));
                myCommand.Parameters.AddWithValue("@description", description);
                myCommand.ExecuteNonQuery();
                myConn.Close();
            }
            catch (Exception ex) { }
        }//Log()

    }
}
