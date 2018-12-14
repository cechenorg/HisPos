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
    class DbConnection
    {
        private SqlConnection _sqlServerConnection;
        private MySqlConnection _sqlMySqlConnection; 
        public DbConnection()
        {
        } 
        public DbConnection(string connection, SqlConnectionType connectionType = SqlConnectionType.SqlServer) {
            switch (connectionType) {
                case SqlConnectionType.SqlServer:
                    _sqlServerConnection = new SqlConnection(connection);
                    break;
                case SqlConnectionType.NySql:
                    _sqlMySqlConnection = new MySqlConnection(connection);
                    break; 
            }  
        }

        public void MySqlNonQueryBySqlString(string sqlString)
        {  
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, _sqlMySqlConnection);
                _sqlMySqlConnection.Open();
                cmd.ExecuteNonQuery();
                _sqlMySqlConnection.Close();
            }
            catch (Exception ex) { 
                switch (ex.Message) {
                    case "建立連接至 SQL Server 時，發生網路相關或執行個體特定的錯誤。找不到或無法存取伺服器。確認執行個名稱是否正確，以及 SQL Server 是否設定為允許遠端連線。 (provider: TCP Provider, error: 0 - 等候操作已逾時。)":
                        MessageWindow messageWindowConnectFail = new MessageWindow("網路異常 無法連線到資料庫",MessageType.ERROR);
                        messageWindowConnectFail.ShowDialog();
                        break;
                    default:
                        MessageWindow messageWindowSQLerror = new MessageWindow("語法失敗\r\n" + sqlString, MessageType.ERROR);
                        messageWindowSQLerror.ShowDialog();
                        break; 
                } 
            }
        }
        public DataTable MySqlQueryBySqlString(string sqlString) {
            var table = new DataTable();
            try
            {
                _sqlMySqlConnection.Open();
                MySqlCommand cmd = new MySqlCommand(sqlString, _sqlMySqlConnection);
                var sqlDapter = new MySqlDataAdapter(cmd);
                sqlDapter.Fill(table);
                _sqlMySqlConnection.Close();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return table;
        }
        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {
            var table = new DataTable();
            try
            {
                _sqlServerConnection.Open();
                var myCommand = new SqlCommand(procName, _sqlServerConnection);
                myCommand.CommandType = CommandType.StoredProcedure;

                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
                _sqlServerConnection.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    switch (ex.Message)
                    {
                        case "建立連接至 SQL Server 時，發生網路相關或執行個體特定的錯誤。找不到或無法存取伺服器。確認執行個名稱是否正確，以及 SQL Server 是否設定為允許遠端連線。 (provider: TCP Provider, error: 0 - 等候操作已逾時。)":
                            MessageWindow messageWindowConnectFail = new MessageWindow("網路異常 無法連線到資料庫", MessageType.ERROR);
                            messageWindowConnectFail.ShowDialog();
                            break;
                        default:
                            MessageWindow messageWindowSQLerror = new MessageWindow("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                            messageWindowSQLerror.ShowDialog();
                            break;
                    }
                });
               
            }
            return table;
        }

        public bool CheckConnection()
        {
            try
            {
                _sqlServerConnection.Open();
            }
            catch (SqlException e)
            {
                return false;
            }

            if (_sqlServerConnection.State == ConnectionState.Open)
            {
                _sqlServerConnection.Close();
                return true;
            }

            return false;
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
            var dd = new DbConnection(Settings.Default.SQL_local);
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("PROC_NAME", procName));
            parameters.Add(new SqlParameter("PROC_PARAM", parameValues));
            var table = dd.ExecuteProc("[HIS_POS_DB].[LOG].[SETPROCLOG]", parameters);
        }//Log()

     
    }
}
