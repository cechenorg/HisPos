using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace His_Pos.NewClass.Trade
{
    public class TradeQueryInfo
    {
        public int CustomerID { get; set; }
        public int TradeID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartInvoice { get; set; }
        public string EndInvoice { get; set; }

        public bool ShowIrregular { get; set; }

        public bool ShowReturn { get; set; }

        public int CashierID { get; set; }

        public string ProID { get; set; }

        public string ProName { get; set; }

        public string Flag { get; set; }

    }
    public class TradeService
    {
        internal static bool Trade(Transaction newTransaction, TradeCustomer customer, TradeEmployee selectedEmployee)
        {
            throw new NotImplementedException();
        }


        public DataTable GetTradeRecord(TradeQueryInfo info)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("MasterID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", info.StartDate));
            parameters.Add(new SqlParameter("eDate", info.EndDate));
            parameters.Add(new SqlParameter("sInvoice", info.StartInvoice));
            parameters.Add(new SqlParameter("eInvoice", info.EndInvoice));
            parameters.Add(new SqlParameter("flag", info.Flag));
            parameters.Add(new SqlParameter("ShowIrregular", info.ShowIrregular));
            parameters.Add(new SqlParameter("ShowReturn", info.ShowReturn));
            parameters.Add(new SqlParameter("Cashier", info.CashierID));
            parameters.Add(new SqlParameter("ProID", info.ProID));
            parameters.Add(new SqlParameter("ProName", info.ProName));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public DataTable GetTradeRecordDetail(TradeQueryInfo info)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parametersDetail = new List<SqlParameter>();
            parametersDetail.Add(new SqlParameter("CustomerID", DBNull.Value));
            parametersDetail.Add(new SqlParameter("MasterID", DBNull.Value));
            parametersDetail.Add(new SqlParameter("sDate", info.StartDate));
            parametersDetail.Add(new SqlParameter("eDate", info.EndDate));
            parametersDetail.Add(new SqlParameter("sInvoice", info.StartInvoice));
            parametersDetail.Add(new SqlParameter("eInvoice", info.EndInvoice));
            parametersDetail.Add(new SqlParameter("flag", "0"));
            parametersDetail.Add(new SqlParameter("ShowIrregular", info.ShowIrregular));
            parametersDetail.Add(new SqlParameter("ShowReturn", info.ShowReturn));
            parametersDetail.Add(new SqlParameter("Cashier", info.CashierID));
            parametersDetail.Add(new SqlParameter("ProID", info.ProID));
            parametersDetail.Add(new SqlParameter("ProName", info.ProName));
            DataTable resultDetail = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDetailQuery]", parametersDetail);
            MainWindow.ServerConnection.CloseConnection();
            return resultDetail;
        }

       
    }
}