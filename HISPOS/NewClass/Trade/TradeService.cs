﻿using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.Person.Customer.ProductTransactionCustomer;
using His_Pos.NewClass.Person.Employee;
using His_Pos.NewClass.Person.Employee.ProductTransaction;
using His_Pos.NewClass.Trade.TradeRecord;
using Newtonsoft.Json;
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

        public float sProfitPercent { get; set; }
        public float eProfitPercent { get; set; }

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
            DataTable table = new DataTable();
            DataColumn dc1 = new DataColumn("Emp_ID", typeof(string));
            DataColumn dc2 = new DataColumn("Emp_Account", typeof(string));
            DataColumn dc3 = new DataColumn("Emp_Name", typeof(string));
            DataColumn dc4 = new DataColumn("Emp_CashierID", typeof(string));
            table.Columns.Add(dc1);
            table.Columns.Add(dc2);
            table.Columns.Add(dc3);
            table.Columns.Add(dc4);
            IEnumerable<Employee> result = GetPosEmployee();
            foreach (Employee item in result)
            {
                DataRow newRow = table.NewRow();
                newRow["Emp_ID"] = Convert.ToString(item.ID);
                newRow["Emp_Account"] = Convert.ToString(item.Account);
                newRow["Emp_Name"] = Convert.ToString(item.Name);
                newRow["Emp_CashierID"] = Convert.ToString(item.CashierID);
                table.Rows.Add(newRow);
            }
            return table;
        }
        public static IEnumerable<Employee> GetPosEmployee()
        {
            IEnumerable<Employee> result = null;
            string sql = string.Format(@"
                select Emp_ID as ID, Emp_Account as Account, Emp_Name as Name, Emp_CashierID as CashierID
                from [{0}].[Employee].[Master]
                where Emp_IsLocal=1 and (Emp_LeaveDate >= GETDATE() Or Emp_LeaveDate is null)
                union
                select 0 as ID, '' as Account, '' as Name,'' as CashierID", Properties.Settings.Default.SystemSerialNumber);
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<Employee>(sql, commandType: CommandType.Text);
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
        //public static DataTable GetTradeRecordSum(TradeQueryInfo info)
        //{
        //    MainWindow.ServerConnection.OpenConnection();
        //    List<SqlParameter> parametersSum = new List<SqlParameter>();
        //    parametersSum.Add(new SqlParameter("CustomerID", DBNull.Value));
        //    parametersSum.Add(new SqlParameter("MasterID", DBNull.Value));
        //    parametersSum.Add(new SqlParameter("sDate", info.StartDate));
        //    parametersSum.Add(new SqlParameter("eDate", info.EndDate));
        //    parametersSum.Add(new SqlParameter("sInvoice", info.StartInvoice));
        //    parametersSum.Add(new SqlParameter("eInvoice", info.EndInvoice));
        //    parametersSum.Add(new SqlParameter("flag", "0"));
        //    parametersSum.Add(new SqlParameter("ShowIrregular", info.ShowIrregular));
        //    parametersSum.Add(new SqlParameter("ShowReturn", info.ShowReturn));
        //    parametersSum.Add(new SqlParameter("Cashier", info.CashierID));
        //    parametersSum.Add(new SqlParameter("ProID", info.ProID));
        //    parametersSum.Add(new SqlParameter("ProName", info.ProName));
        //    DataTable resultSum = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordSum]", parametersSum);
        //    MainWindow.ServerConnection.CloseConnection();
        //    return resultSum;
        //}

        /// <summary>
        /// 銷售彙總
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static DataTable GetTradeRecordSum(TradeQueryInfo info)
        {
            DataTable table = new DataTable();
            string sql = string.Format(@"
                Declare @sDate datetime = '{1}'
                Declare @eDate datetime = '{2}'
                Declare @sInvoice nvarchar(20) = '{3}'
                Declare @eInvoice nvarchar(20) = '{4}'
                Declare @ShowIrregular bit = {5}
                Declare @ShowReturn bit = {6}
                Declare @Cashier int = {7}
                Declare @ProID nvarchar(20) = '{8}'
                Declare @ProName nvarchar(50) = '{9}'
                Declare @sProfitPercent float = {10}
                Declare @eProfitPercent float = {11}
                Declare @IsAvgCost nvarchar(1) = (Select SysPar_Value From [{0}].[SystemInfo].[SystemParameters] Where SysPar_Name = 'AvgCost')

                SELECT TM.TraMas_ID,TD.TraDet_ProductID, M.Pro_ChineseName as TraDet_ProductName,TD.TraDet_Amount,TD.TraDet_PriceSum
                into #tempTra
                FROM [{0}].[POS].[TradeMaster] TM WITH (NOLOCK)
                  Inner Join [{0}].[POS].[TradeDetails] TD WITH (NOLOCK) on TM.TraMas_ID = TD.TraDet_MasterID
                  Inner Join [{0}].[Product].[Master] M WITH (NOLOCK) on M.Pro_ID = TD.TraDet_ProductID
                  Left Join [{0}].[Employee].[Master] E WITH (NOLOCK) on TM.TraMas_Cashier = cast(E.Emp_CashierID as nvarchar)					 
                WHERE cast(TraMas_ChkoutTime as Date) between @sDate and @eDate 
                  and (try_convert(int, SUBSTRING([TraMas_InvoiceNumber], 3,8)) >=try_convert(int, @sInvoice) or @sInvoice IS NULL OR @sInvoice='') 
                  and (try_convert(int, SUBSTRING([TraMas_InvoiceNumber], 3,8)) <=try_convert(int, @eInvoice) or @eInvoice IS NULL OR @eInvoice='') 
                  and (Emp_ID = @Cashier or @Cashier is null or cast(@Cashier as nvarchar)=''or cast(@Cashier as nvarchar)=-1) 
                  and TD.TraDet_PriceSum >= 0 
                  and (TD.TraDet_ProductID like '%'+@ProID+'%' or @ProID is null or @ProID='') 
                  and (M.Pro_ChineseName like '%'+@ProName+'%' or @ProName is null or @ProName='') 
                  and ((@ShowReturn = 0 and TM.TraMas_IsEnable = 1) or (@ShowReturn = 1 and TM.TraMas_IsEnable = 0)) 
                  and (@ShowIrregular = 0 or (@ShowIrregular = 1 and (TraMas_DiscountAmt <> 0 or TD.TraDet_IsGift = 1)))

				SELECT ProID,InvID INTO #ProdInv
				FROM
				( SELECT ProInv_ProductID as ProID,ProInv_InventoryID as InvID
				  FROM [{0}].[Product].[ProductInventory] WITH(NOLOCK)
				  WHERE ProInv_ProductID IN (SELECT TraDet_ProductID FROM #tempTra) AND ProInv_WareHouseID = 0
				  UNION 
				  SELECT MerSplRec_ProductID as ProID,MerSplRec_OldInvID as InvID
				  FROM [{0}].[Product].[MergeSplitRecord] WITH(NOLOCK)			   
				  WHERE MerSplRec_ProductID IN (SELECT TraDet_ProductID FROM #tempTra) AND MerSplRec_WareHouseID = 0
				  UNION
				  SELECT MerSplRec_ProductID as ProID,MerSplRec_NewInvID as InvID
				  FROM [{0}].[Product].[MergeSplitRecord] WITH(NOLOCK)			   
				  WHERE MerSplRec_ProductID IN (SELECT TraDet_ProductID FROM #tempTra) AND MerSplRec_WareHouseID = 0
				) pinv
								
				SELECT inv.SourceID, inv.InvRec_InventoryID
				,CASE @IsAvgCost WHEN '0' THEN ISNULL(inv.CostValue,0) WHEN '1' THEN ISNULL(invMA.CostValue,inv.CostValue) END AS CostValue
				,CASE @IsAvgCost WHEN '0' THEN ISNULL(inv.ChangeStock,0) WHEN '1' THEN ISNULL(invMA.ChangeStock,inv.ChangeStock) END AS ChangeStock
				INTO #tempCost
				FROM 
				(	SELECT i.InvRecSourceID AS SourceID, i.InvRec_InventoryID, SUM(ISNULL(i.InvRec_ValueDifference,0)) AS CostValue
						,SUM((i.InvRec_OldStock - i.InvRec_NewStock)) AS ChangeStock
					FROM [{0}].[Product].[InventoryRecord] i WITH (NOLOCK)
					WHERE Cast(InvRec_Time as date) between @sDate and @eDate
					  and i.InvRec_Source = 'TraMasId'
					  and i.InvRec_Type not in ('銷售退貨', '銷售刪單')
					  and i.InvRecSourceID  in (SELECT DISTINCT TraMas_ID from #tempTra)
					  and i.InvRec_InventoryID in (SELECT DISTINCT InvID FROM #ProdInv )
					GROUP BY i.InvRecSourceID, i.InvRec_InventoryID
				) inv
				LEFT JOIN 
				(	SELECT im.InvRec_SourceID AS SourceID, im.InvRec_InventoryID, SUM(ISNULL(im.InvRec_ChangeValue,0)) AS CostValue
						,SUM(ABS(im.InvRec_ChangeStock)) AS ChangeStock 
                	FROM [{0}].[Product].[InventoryRecordMA] im WITH (NOLOCK)
                	WHERE Cast(InvRec_Time as date) between @sDate and @eDate
                			and InvRec_Source = 'TraMasId'
                			and InvRec_Type not in ('銷售退貨', '銷售刪單') 
							and im.InvRec_SourceID in (SELECT DISTINCT TraMas_ID from #tempTra) 
							and InvRec_InventoryID in (SELECT DISTINCT InvID FROM #ProdInv )
					GROUP BY im.InvRec_SourceID, im.InvRec_InventoryID
                ) invMA ON inv.SourceID = invMA.SourceID AND inv.InvRec_InventoryID = invMA.InvRec_InventoryID

				
				SELECT t.TraDet_ProductID, t.TraDet_ProductName, SUM(t.TraDet_Amount) as TraDet_Amount, SUM(t.TraDet_PriceSum) as TraDet_PriceSum,SUM(ISNULL(tc.CostValue,0)) AS InvRec_ValueDifference 
				,SUM(ISNULL(tc.ChangeStock,0)) ChangeStock
				INTO #tempTraCost
				FROM (	SELECT TraMas_ID, TraDet_ProductID,TraDet_ProductName, SUM(TraDet_Amount) as TraDet_Amount, SUM(TraDet_PriceSum) as TraDet_PriceSum
                		FROM #tempTra GROUP BY TraMas_ID, TraDet_ProductID,TraDet_ProductName ) t
				LEFT JOIN 
					 (	SELECT t.TraMas_ID,t.TraDet_ProductID,SUM(ISNULL(c.CostValue,0)) CostValue,SUM(ISNULL(c.ChangeStock,0)) ChangeStock
						FROM ( SELECT DISTINCT TraMas_ID, TraDet_ProductID FROM #tempTra ) t
						LEFT JOIN #ProdInv pinv ON t.TraDet_ProductID = pinv.ProID 
						LEFT JOIN #tempCost c ON t.TraMas_ID = c.SourceID AND pinv.InvID = c.InvRec_InventoryID 
						GROUP BY t.TraMas_ID,t.TraDet_ProductID
					  ) tc ON t.TraMas_ID = tc.TraMas_ID AND t.TraDet_ProductID = tc.TraDet_ProductID 
				GROUP BY t.TraDet_ProductID, t.TraDet_ProductName

                drop table #tempTra
				drop table #ProdInv
                drop table #tempCost

                SELECT TraDet_ProductID, TraDet_ProductName, TraDet_Amount, TraDet_PriceSum, ABS(InvRec_ValueDifference) as TotalCost,
                (TraDet_PriceSum+InvRec_ValueDifference) Profit, case when Isnull(TraDet_PriceSum, 0) <> 0 then (TraDet_PriceSum+InvRec_ValueDifference)/TraDet_PriceSum else (TraDet_PriceSum+InvRec_ValueDifference)/1 end ProfitPercent
                FROM #tempTraCost
                WHERE (@sProfitPercent = -1 and @eProfitPercent = 1) or--全部顯示
                      (@sProfitPercent = 0 and @eProfitPercent = 1 and case when Isnull(TraDet_PriceSum, 0) <> 0 then (TraDet_PriceSum+InvRec_ValueDifference)/TraDet_PriceSum else 0 end > 0 ) or--正毛利
                      (@sProfitPercent = -1 and @eProfitPercent = -1 and case when Isnull(TraDet_PriceSum, 0) <> 0 then (TraDet_PriceSum+InvRec_ValueDifference)/TraDet_PriceSum else 0 end < 0) or--顯示負毛利
                      (case when Isnull(TraDet_PriceSum, 0) <> 0 then (TraDet_PriceSum+InvRec_ValueDifference)/TraDet_PriceSum else 0 end between @sProfitPercent and @eProfitPercent)--區間查詢
                ORDER BY ProfitPercent DESC,Profit DESC

                drop table #tempTraCost",
                Properties.Settings.Default.SystemSerialNumber, info.StartDate, info.EndDate, info.StartInvoice, info.EndInvoice, info.ShowIrregular ? 1 : 0, info.ShowReturn ? 1 : 0, info.CashierID, info.ProID, info.ProName, info.sProfitPercent,info.eProfitPercent);
            SQLServerConnection.DapperQuery((conn) =>
            {
                var dapper = conn.Query(sql, commandType: CommandType.Text);
                string json = JsonConvert.SerializeObject(dapper);//序列化成JSON
                table = JsonConvert.DeserializeObject<DataTable>(json);//反序列化成DataTable
            });
            return table;
        }


        public static DataTable GetTradeRecordCusSum(TradeQueryInfo info)
        {
            DataTable table = new DataTable();
            string sql = string.Format(@"
                Declare @sdate date = '{1}'
                Declare @edate date = '{2}'
                Declare @sInvoice nvarchar(15) = '{3}'
                Declare @eInvoice nvarchar(15) = '{4}'
                Declare @IsReturn bit = {5}
                Declare @ProID nvarchar(20) = '{6}'
                Declare @ProName nvarchar(50) = '{7}'
                Declare @sProfitPercent float = {8}
                Declare @eProfitPercent float = {9}
                Declare @Cashier int = {10}
                Declare @IsAvgCost int = (Select SysPar_Value From [{0}].[SystemInfo].[SystemParameters] Where SysPar_Name = 'AvgCost')
                
                if @sInvoice is null
	            Begin
                    Set @sInvoice = 0;
                End

	            Select f.Cus_Name, Count(a.TraMas_ID) as TraCount, Sum(tm.TraDet_PriceSum) as TraMas_RealTotal, 
	                case when @IsAvgCost = 1 then (Sum(tm.TraDet_PriceSum)+Sum(c.InvRec_ChangeValue)) 
	                else (Sum(tm.TraDet_PriceSum)+Sum(b.InvRec_ValueDifference)) end as Profit, 
	                case when @IsAvgCost = 1 then (Round(Cast((Sum(tm.TraDet_PriceSum)+Sum(c.InvRec_ChangeValue)) as float)/Case When Sum(tm.TraDet_PriceSum) = 0 then 1 else Sum(tm.TraDet_PriceSum) end, 4)) 
	                else (Round(Cast((Sum(tm.TraDet_PriceSum)+Sum(b.InvRec_ValueDifference)) as float)/Case When Sum(tm.TraDet_PriceSum) = 0 then 1 else Sum(tm.TraDet_PriceSum) end, 4)) end as ProfitPercent
	            into #temp
	            From [{0}].[Pos].[TradeMaster] a
                Inner Join (Select TraDet_MasterID, TraDet_ProductID, Sum(TraDet_PriceSum) as TraDet_PriceSum From [{0}].[Pos].[TradeDetails] Group By TraDet_MasterID, TraDet_ProductID) tm on a.TraMas_ID = tm.TraDet_MasterID
                Inner Join [{0}].[Product].[Master] pm on tm.TraDet_ProductID = pm.Pro_ID
                Inner Join [{0}].[Product].[ProductInventory] pp on tm.TraDet_ProductID = pp.ProInv_ProductID and pp.ProInv_WareHouseID = case when pm.Pro_TypeID = 4 then 9 else 0 end
                Inner Join (Select InvRecSourceID, Sum(InvRec_ValueDifference) as InvRec_ValueDifference, InvRec_InventoryID From [{0}].[Product].[InventoryRecord] Where InvRec_Source = 'TraMasId' and InvRec_Type not in ('銷售退貨', '銷售刪單') and cast(InvRec_Time as date) between @sdate and @edate Group By InvRecSourceID, InvRec_InventoryID) b on Convert(nvarchar(20), a.TraMas_ID) = b.InvRecSourceID and pp.ProInv_InventoryID = b.InvRec_InventoryID
                Inner Join (Select InvRec_SourceID, Sum(InvRec_ChangeValue) as InvRec_ChangeValue, InvRec_InventoryID From [{0}].[Product].[InventoryRecordMA] Where InvRec_Source = 'TraMasId' and InvRec_Type not in ('銷售退貨', '銷售刪單') and cast(InvRec_Time as date) between @sdate and @edate Group By InvRec_SourceID, InvRec_InventoryID) c on Convert(nvarchar(20), a.TraMas_ID) = c.InvRec_SourceID and pp.ProInv_InventoryID = c.InvRec_InventoryID
                Inner Join [{0}].[Customer].[Master] f on a.TraMas_CustomerID = f.Cus_ID
	            Where 
                    tm.TraDet_ProductID <> 'PREPAY' and
		            Cast(a.TraMas_ChkoutTime as date) between @sdate and @edate and
		            (TRY_CAST(substring(a.TraMas_InvoiceNumber, 3,8) as int)>=TRY_CAST(@sInvoice as int) OR @sInvoice is null or @sInvoice = '') and
		            (TRY_CAST(substring(a.TraMas_InvoiceNumber, 3,8) as int)<=TRY_CAST(@eInvoice as int) OR @eInvoice is null or @eInvoice = '') and
                    (a.TraMas_Cashier = @Cashier or @Cashier is null or @Cashier=''or @Cashier=-1) and 
                    (tm.TraDet_ProductID like '%'+@ProID+'%' or @ProID is null or @ProID='') and
	                (pm.Pro_ChineseName like '%'+@ProName+'%' or @ProName is null or @ProName='') and 
		            ((@IsReturn = 0 and a.TraMas_IsEnable = 1) Or (@IsReturn = 1 and TraMas_IsEnable = 0 and a.TraMas_UpdateTime is not null))
	            Group By f.Cus_Name
	            Order By Profit Desc
	
	            Select * From #temp
	            Where
                    (@sProfitPercent = -1 and @eProfitPercent = 1) or--全部顯示
					(@sProfitPercent = 0 and @eProfitPercent = 1 and ProfitPercent > 0 ) or--正毛利
					(@sProfitPercent = -1 and @eProfitPercent = -1 and ProfitPercent < 0) or--顯示負毛利
					(ProfitPercent between @sProfitPercent and @eProfitPercent)--區間查詢
	            Order By ProfitPercent Desc
            ", Properties.Settings.Default.SystemSerialNumber, info.StartDate, info.EndDate, info.StartInvoice, info.EndInvoice, info.ShowReturn ? 1 : 0, info.ProID, info.ProName, info.sProfitPercent, info.eProfitPercent, info.CashierID);
            SQLServerConnection.DapperQuery((conn) =>
            {
                var dapper = conn.Query(sql, commandType: CommandType.Text);
                string json = JsonConvert.SerializeObject(dapper);//序列化成JSON
                table = JsonConvert.DeserializeObject<DataTable>(json);//反序列化成DataTable
            });
            return table;
        }
    }
}