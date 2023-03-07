using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using His_Pos.NewClass.Trade.TradeRecord;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public static DataTable GetTradeRecord(int traMas_ID)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("MasterID", traMas_ID),
                    new SqlParameter("CustomerID", DBNull.Value),
                    new SqlParameter("sDate", ""),
                    new SqlParameter("eDate", ""),
                    new SqlParameter("sInvoice", ""),
                    new SqlParameter("eInvoice", ""),
                    new SqlParameter("flag", "1"),
                    new SqlParameter("ShowIrregular", DBNull.Value),
                    new SqlParameter("ShowReturn", DBNull.Value),
                    new SqlParameter("Cashier", -1),
                    new SqlParameter("ProID", DBNull.Value),
                    new SqlParameter("ProName", DBNull.Value)
                };
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            return result;
        }
        /// <summary>
        /// 銷售明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DataTable GetTradeRecordDetail(TradeQueryInfo info)
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
            IEnumerable<Employee> empList = GetPosEmployee();
            foreach (var item in detail)
            {
                IEnumerable<Employee> emp = empList.Where(w => w.Name == Convert.ToString(item.Emp.Name));

                string Id = Convert.ToString(emp.First().ID) == "0" ? null : Convert.ToString(emp.First().ID);
                string rewardPercent = item.TraDet_RewardPercent == 0 || item.TraDet_Amount == 0 ? null : Convert.ToString(Math.Round(Convert.ToDouble(item.TraDet_RewardPercent / item.TraDet_Amount), 0));
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
                    rewardPercent);
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
            DataTable table = new DataTable();
            DataColumn dc1 = new DataColumn("Emp_ID", typeof(string));
            DataColumn dc2 = new DataColumn("Emp_Account", typeof(string));
            DataColumn dc3 = new DataColumn("Emp_Name", typeof(string));
            DataColumn dc4 = new DataColumn("Emp_CashierID", typeof(string));
            table.Columns.Add(dc1);
            table.Columns.Add(dc2);
            table.Columns.Add(dc3);
            table.Columns.Add(dc4);
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[GetEmployee]");
            foreach (DataRow dr in result.Rows)
            {
                DataRow newRow = table.NewRow();
                newRow["Emp_ID"] = Convert.ToString(dr["ID"]);
                newRow["Emp_Account"] = Convert.ToString(dr["Account"]);
                newRow["Emp_Name"] = Convert.ToString(dr["Name"]);
                newRow["Emp_CashierID"] = Convert.ToString(dr["CashierID"]);
                table.Rows.Add(newRow);
            }
            
            MainWindow.ServerConnection.CloseConnection();
            return table;
        }
        public static IEnumerable<Employee> GetPosEmployee()
        {
            IEnumerable<Employee> result = null;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Employee>($"{Properties.Settings.Default.SystemSerialNumber}.[POS].[GetEmployee]",
                    commandType: CommandType.StoredProcedure).ToList();
            });

            return result;
        }
        /// <summary>
        /// 銷售紀錄
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DataTable GetTradeRecordTable(TradeQueryInfo info)
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
        /// <summary>
        /// 銷售彙總
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DataTable GetTradeRecordSum(TradeQueryInfo info)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parametersSum = new List<SqlParameter>();
            parametersSum.Add(new SqlParameter("CustomerID", DBNull.Value));
            parametersSum.Add(new SqlParameter("MasterID", DBNull.Value));
            parametersSum.Add(new SqlParameter("sDate", info.StartDate));
            parametersSum.Add(new SqlParameter("eDate", info.EndDate));
            parametersSum.Add(new SqlParameter("sInvoice", info.StartInvoice));
            parametersSum.Add(new SqlParameter("eInvoice", info.EndInvoice));
            parametersSum.Add(new SqlParameter("flag", "0"));
            parametersSum.Add(new SqlParameter("ShowIrregular", info.ShowIrregular));
            parametersSum.Add(new SqlParameter("ShowReturn", info.ShowReturn));
            parametersSum.Add(new SqlParameter("Cashier", info.CashierID));
            parametersSum.Add(new SqlParameter("ProID", info.ProID));
            parametersSum.Add(new SqlParameter("ProName", info.ProName));
            DataTable resultSum = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordSum]", parametersSum);
            MainWindow.ServerConnection.CloseConnection();
            return resultSum;
        }
    }
}