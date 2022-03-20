using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading;

namespace His_Pos.Database
{
    public class MySQLConnection : DatabaseConnection
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
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
               {
                   NewFunction.ExceptionLog(e.Message);
                   MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
               });
            }
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public DataTable ExecuteProc(string sqlString)
        {
            while (connection.State == ConnectionState.Executing)
                Thread.Sleep(1000);

            var table = new DataTable();
            try
            {
                MySqlCommand cmd = new MySqlCommand(sqlString, connection);
                cmd.CommandTimeout = 120;
                var sqlDapter = new MySqlDataAdapter(cmd);
                sqlDapter.Fill(table);
            }
            catch (Exception ex)
            {
                NewFunction.ExceptionLog(ex.Message);
            }

            return table;
        }

        public ConnectionState ConnectionStatus()
        {
            return connection.State;
        }
    }
}