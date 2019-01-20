using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using His_Pos.Class;
using His_Pos.FunctionWindow;

namespace His_Pos.Database
{
    public class SQLServerConnection : DatabaseConnection
    {
        private SqlConnection connection = new SqlConnection(Properties.Settings.Default.SQL_local);

        public bool CheckConnection()
        {
            try
            {
                connection.Open();
            }
            catch (SqlException e)
            {
                return false;
            }

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                return true;
            }

            return false;
        }


        public void OpenConnection()
        {
            try
            {
                if(connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage("網路異常 無法連線到資料庫", MessageType.ERROR);
            }
        }

        public void CloseConnection()
        {
            if( connection.State != ConnectionState.Executing && connection.State != ConnectionState.Fetching && connection.State == ConnectionState.Open)
            connection.Close();
        }

        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {
            if (connection.State != ConnectionState.Open) {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage("Plz Open Connection when execute " + procName, MessageType.ERROR);
                    return;
                });
            }

            var table = new DataTable();
            try
            {
                var myCommand = new SqlCommand("[" + Properties.Settings.Default.SystemSerialNumber + "]." +  procName, connection);
                myCommand.CommandType = CommandType.StoredProcedure;

                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
            }
            catch (SqlException sqlException)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage(procName + sqlException.Message, MessageType.ERROR);
                });
            }
            catch (Exception ex)
            {
                string parameValues = string.Empty;
                foreach (SqlParameter row in parameterList)
                {
                    parameValues += row.ParameterName + ":" + row.Value.ToString() + "\r\n";
                }

                LogError(procName, parameValues, ex.Message);

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }
            return table;
        }
        
        private void LogError(string procName, string parameters, string error)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("NAME", procName));
            parameterList.Add(new SqlParameter("PARAM", parameters));
            parameterList.Add(new SqlParameter("ERROR", error));
            ExecuteProc("[HIS_POS_DB].[LOG].[InsertProcLog]", parameterList);
        }

    }
}
