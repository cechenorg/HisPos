using Dapper;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;

namespace His_Pos.Database
{
    public class SQLServerConnection : DatabaseConnection
    {
        private SqlConnection connection = new SqlConnection(Properties.Settings.Default.SQL_local);
        private bool isBusy = false;

        public bool CheckConnection()
        {
            try
            {
                connection.Open();
            }
            catch (SqlException)
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
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            catch (Exception)
            {
                MessageWindow.ShowMessage("網路異常 無法連線到資料庫", MessageType.ERROR);
            }
        }

        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Executing && connection.State != ConnectionState.Fetching && connection.State == ConnectionState.Open)
                connection.Close();
        }

        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null,string dbName = null)
        {
            while (isBusy)
                Thread.Sleep(500);

            isBusy = true;

            if (dbName == null)
                dbName =  Properties.Settings.Default.SystemSerialNumber;

            var table = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand("[" + dbName + "]." + procName, connection);

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandTimeout = 120;
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
                NewFunction.ExceptionLog(sqlException.Message);
            }
            catch (Exception ex)
            {
                NewFunction.ExceptionLog(ex.Message);

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }

            isBusy = false;

            return table;
        }

        public DataTable ExecuteProcBySchema(string schema, string procName, List<SqlParameter> parameterList = null)
        {
            while (isBusy)
                Thread.Sleep(500);

            isBusy = true;

            var table = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand("[" + schema + "]." + procName, connection);

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandTimeout = 120;
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
                NewFunction.ExceptionLog(sqlException.Message);
            }
            catch (Exception ex)
            {
                NewFunction.ExceptionLog(ex.Message);

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }

            isBusy = false;

            return table;
        }

        public DataSet ExecuteProcReturnDataSet(string procName, List<SqlParameter> parameterList = null)
        {
            while (isBusy)
                Thread.Sleep(500);

            isBusy = true;

            DataSet dataSet = new DataSet();
            try
            {
                SqlCommand myCommand = new SqlCommand("[" + Properties.Settings.Default.SystemSerialNumber + "]." + procName, connection);

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandTimeout = 120;
                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                sqlDapter.Fill(dataSet);
            }
            catch (SqlException sqlException)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage(procName + sqlException.Message, MessageType.ERROR);
                });
                NewFunction.ExceptionLog(sqlException.Message);
            }
            catch (Exception ex)
            {
                NewFunction.ExceptionLog(ex.Message);

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }

            isBusy = false;

            return dataSet;
        }

        public static IEnumerable<T> Query<T>(string connectionString,string dbName,string procedureName,object param = null)
        {

            using (var dconn = new SqlConnection(connectionString))
            {

                dconn.Open(); 
                var result = dconn.Query<T>($"{dbName}.{procedureName}",param,commandType : CommandType.StoredProcedure);
                dconn.Close();
                 
                return result;
            } 
        }


        public SqlConnection GetConnection()
        {
            return connection;
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