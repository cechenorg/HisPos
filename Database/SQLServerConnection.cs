﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.Properties;

namespace His_Pos.Database
{
    public class SQLServerConnection : DatabaseConnection
    {
        private SqlConnection connection = new SqlConnection(Settings.Default.SQL_local);
        public SQLServerConnection() {}

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

        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {
            var table = new DataTable();
            try
            {
                var myCommand = new SqlCommand(procName, connection);
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
            catch (Exception ex)
            {
                string parameValues = string.Empty;
                foreach (SqlParameter row in parameterList)
                {
                    parameValues += row.ParameterName + ":" + row.Value.ToString() + "\r\n";
                }

                LogError(procName, parameValues, ex.Message);

                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageWindow messageWindowSQLerror = new MessageWindow("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                    messageWindowSQLerror.ShowDialog();
                });
            }
            return table;
        }


        public void LogError(string procName, string parameters, string error)
        {
            var parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("NAME", procName));
            parameterList.Add(new SqlParameter("PARAM", parameters));
            parameterList.Add(new SqlParameter("ERROR", error));
            ExecuteProc("[HIS_POS_DB].[LOG].[InsertProcLog]", parameterList);
        }

        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                MessageWindow messageWindowConnectFail = new MessageWindow("網路異常 無法連線到資料庫", MessageType.ERROR);
                messageWindowConnectFail.ShowDialog();
            }
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
}