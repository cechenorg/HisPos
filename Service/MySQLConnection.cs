using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class;
using MySql.Data.MySqlClient;

namespace His_Pos.Service
{
    public class MySQLConnection: DatabaseConnection
    {
        private MySqlConnection _sqlMySqlConnection;

        public void OpenConnection()
        {
            throw new NotImplementedException();
        }

        public void CloseConnection()
        {
            throw new NotImplementedException();
        }

        public void LogError(string procName, string parameters, string error)
        {
            throw new NotImplementedException();
        }


        public DataTable MySqlNonQueryBySqlString(string sqlString)
        {
            var table = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, _sqlMySqlConnection);
                var sqlDapter = new MySqlDataAdapter(cmd);
                sqlDapter.Fill(table);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "建立連接至 SQL Server 時，發生網路相關或執行個體特定的錯誤。找不到或無法存取伺服器。確認執行個名稱是否正確，以及 SQL Server 是否設定為允許遠端連線。 (provider: TCP Provider, error: 0 - 等候操作已逾時。)":
                        MessageWindow messageWindowConnectFail = new MessageWindow("網路異常 無法連線到資料庫", MessageType.ERROR);
                        messageWindowConnectFail.ShowDialog();
                        break;
                    default:
                        MessageWindow messageWindowSQLerror = new MessageWindow("語法失敗\r\n" + sqlString, MessageType.ERROR);
                        messageWindowSQLerror.ShowDialog();
                        break;
                }
            }

            return table;
        }
        public DataTable MySqlQueryBySqlString(string sqlString)
        {
            var table = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, _sqlMySqlConnection);
                var sqlDapter = new MySqlDataAdapter(cmd);
                sqlDapter.Fill(table);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            return table;
        }
    }
}
