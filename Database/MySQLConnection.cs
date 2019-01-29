using System;
using System.Data;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using MySql.Data.MySqlClient;

namespace His_Pos.Database
{
    public class MySQLConnection: DatabaseConnection
    {
        private MySqlConnection connection = new MySqlConnection(Properties.Settings.Default.SingdeServer);

        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage("網路異常 無法連線到資料庫", MessageType.ERROR);
            }
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public DataTable ExecuteProc(string sqlString)
        {
            var table = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                var sqlDapter = new MySqlDataAdapter(cmd);
                sqlDapter.Fill(table);
            }
            catch (Exception ex)
            {
                NewFunction.ExceptionLog(ex.Message);
            }

            return table;
        }
    }
}
