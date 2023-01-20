using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using His_Pos.NewClass.Trade.TradeRecord;
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

       public static DataTable TradeRecordDelete(string masID)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", masID));
            parameters.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordDelete]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
            
        }
        public static DataTable TradeRecordInsert(string masID, string cusID, string payMethod, int preTotal,int realTotal, int discountAmt, string cardNum, string invoiceNum, string taxNum, string cashier, string note, double cash, double card, double voucher, double cashCoupon, List<TradeDetail> detail)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("ID", masID));
            parameters.Add(new SqlParameter("CustomerID", cusID));
            parameters.Add(new SqlParameter("PayMethod", payMethod));
            parameters.Add(new SqlParameter("PreTotal", preTotal));
            parameters.Add(new SqlParameter("DiscountAmt", discountAmt));
            parameters.Add(new SqlParameter("RealTotal", realTotal));
            parameters.Add(new SqlParameter("CardNumber", cardNum));
            parameters.Add(new SqlParameter("InvoiceNumber", invoiceNum));
            parameters.Add(new SqlParameter("TaxNumber", taxNum));
            parameters.Add(new SqlParameter("Cashier", cashier));
            parameters.Add(new SqlParameter("Note", note));
            parameters.Add(new SqlParameter("TraMas_CashAmount", cash));
            parameters.Add(new SqlParameter("TraMas_CardAmount", card));
            parameters.Add(new SqlParameter("TraMas_VoucherAmount", voucher));
            parameters.Add(new SqlParameter("TraMas_CashCoupon", cashCoupon));
            parameters.Add(new SqlParameter("DETAILS", TransferDetailTable(detail)));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordEdit]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
        private static DataTable TransferDetailTable(List<TradeDetail> detail)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TraDet_DetailID", typeof(int));
            dt.Columns.Add("TraDet_ProductID", typeof(string));
            dt.Columns.Add("TraDet_Amount", typeof(int));
            dt.Columns.Add("TraDet_PriceType", typeof(string));
            dt.Columns.Add("TraDet_Price", typeof(int));
            dt.Columns.Add("TraDet_PriceSum", typeof(int));
            dt.Columns.Add("TraDet_IsGift", typeof(int));
            dt.Columns.Add("TraDet_DepositAmount", typeof(int));
            dt.Columns.Add("TraDet_RewardPersonnel", typeof(string));
            dt.Columns.Add("TraDet_RewardPercent", typeof(int));
            DataTable empList = GetEmployeeList();
            foreach (var item in detail)
            {
                DataRow[] drs = empList.Select(string.Format("Emp_Name = '{0}'", Convert.ToString(item.Emp.Emp_Name)));
                string Id = Convert.ToString(drs[0]["Emp_ID"]);

                dt.Rows.Add(
                    item.TraDet_DetailID,
                    item.TraDet_ProductID,
                    item.TraDet_Amount,
                    item.TraDet_PriceType,
                    item.TraDet_Price,
                    item.TraDet_PriceSum,
                    item.TraDet_IsGift,
                    item.TraDet_DepositAmount,
                    Id,
                    item.TraDet_RewardPercent);
            }
            return dt;
        }

        public static DataTable TradeRecordReturn(string masID)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", masID));
            parameters.Add(new SqlParameter("Emp", ViewModelMainWindow.CurrentUser.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordReturn]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
        public static DataTable GetPriceList(string id)
        {
            int war = 0;
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("SEARCH_STRING", id));
            parameters.Add(new SqlParameter("WAREHOUSE_ID", war));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[SearchProductPriceByID]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
        public static DataTable GetEmployeeList()
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
    }
}