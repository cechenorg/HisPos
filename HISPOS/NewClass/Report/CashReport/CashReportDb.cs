using Dapper;
using His_Pos.ChromeTabViewModel;
using His_Pos.Database;
using His_Pos.NewClass.BalanceSheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.CashReport
{
    public static class CashReportDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportByDate]", parameterList);
        }

        internal static DataTable GetClosingHistories(DateTime beginDate, DateTime endDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", beginDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkHistory]", parameterList);
        }

        internal static DataSet GetYearIncomeStatementForExport(int year)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "YEAR", year);
            return MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[YearIncomeStatementForExport]", parameterList);
        }

        public static DataTable GetPerDayDataByDate(DateTime sDate, DateTime eDate, string insID)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            DataBaseFunction.AddSqlParameter(parameterList, "InsID", insID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashReportPerDayByID]", parameterList);
        }

        internal static DataSet GetBalanceSheet()
        {
            return MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[BalanceSheet]");
        }

        internal static DataTable StrikeBalanceSheet(string strikeType, string sheetType, double value, string sourceID, string note = "")//BalanceSheetTypeEnum
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("VALUE", value));
            parameters.Add(new SqlParameter("TYPE", sheetType.ToString()));
            parameters.Add(new SqlParameter("NOTE", note));
            parameters.Add(new SqlParameter("TARGET", strikeType));
            parameters.Add(new SqlParameter("SOURCE_ID", sourceID));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[StrikeBalanceSheet]", parameters);
        }

        internal static DataTable SetDeclareDoneMonth(DateTime dateTime)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("DATE", dateTime));

            return MainWindow.ServerConnection.ExecuteProc("[Set].[DeclareDoneMonth]", parameters);
        }

        internal static DataTable GetStrikeHistories()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StrikeHistoriesBySource]");
        }
        internal static DataTable GetSelectStrikeHistories(string type, DateTime sdate, DateTime edate)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            if(!string.IsNullOrEmpty(type))
                parameters.Add(new SqlParameter("type", type));
            parameters.Add(new SqlParameter("sdate", sdate));
            parameters.Add(new SqlParameter("edate", edate));
            return MainWindow.ServerConnection.ExecuteProc("[Get].[StrikeHistoriesBySourceByDate]", parameters);
        }


        public static void DeleteStrikeHistory(StrikeHistory selectedHistory)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP_ID", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Values", selectedHistory.StrikeValue));
            parameters.Add(new SqlParameter("StrikeSource", selectedHistory.StrikeSource));
            parameters.Add(new SqlParameter("StrikeID", selectedHistory.StrikeID));
            parameters.Add(new SqlParameter("Source", selectedHistory.StrikeWay));
            parameters.Add(new SqlParameter("StrikeSourceID", selectedHistory.StrikeSourceID));
            parameters.Add(new SqlParameter("StrikeNote", selectedHistory.StrikeNote));
            MainWindow.ServerConnection.ExecuteProc("[Set].[DeleteStrikeHistory]", parameters);
        }

        public static DataTable GetInventoryDifferenceByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "SDATE", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "EDATE", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[InventoryDifferanceByDate]", parameterList);
        }

        public static DataTable GetExportCashData(DateTime sdate, DateTime edate)
        {
            DataTable table = new DataTable();
            string sql = string.Format(@"
                select
                    Total.Id,PreMas_AdjustCaseID,
                    Isnull(Ins_Name,'無') Ins_Name,
                    Isnull(Cus_Name,'無') Cus_Name,
                    Cast(Isnull(部分負擔,0) as int) CopayMentPrice,
                    Cast(Isnull(自費,0)as int) PaySelfPrice,
                    Cast(Isnull(自費調劑,0)as int) PaySelfPrecriptionPrice,
                    Cast(Isnull(押金,0)as int) Deposit,
                    Cast(Isnull(其他,0)as int) Other
                    from(
                        select *
                        from(
                            Select
                            Isnull(pre.PreMas_ID,0) Id,[PreMas_AdjustCaseID],
                            PreMas_CustomerID,
                            PreMas_InstitutionID,
                            PreMas_DivisionID,
                            Case When CashFlow_Name like '%部分負擔%' Then '部分負擔'
                            When CashFlow_Name like '%自費%' and CashFlow_Name not like '%自費調劑%' Then '自費'
                            When CashFlow_Name like '%自費調劑%' Then '自費調劑'
                            When CashFlow_Name like '%押金%' Then '押金'
                            When CashFlow_Name = '' Then '其他' End CashFlow_Name,
                            --Case when Ins_ID in (select CooCli_ID from [{0}].[His].[CooperativeClinic]) Then Ins_Name Else '排除合作' End Ins_Name,
                            Ins_Name,
                            CashFlow_Value
                            from [{0}].[Report].[CashFlow] cash inner join [{0}].[His].[PrescriptionMaster] pre on CashFlow_Source = 'PreMasId' and　CashFlow_SourceID = cast(pre.PreMas_ID as nvarchar )
                            left join [{0}].[dbo].[View_PrescriptionCooCliType] View_PreCooCli on View_PreCooCli.PreMas_ID = pre.PreMas_ID
                            left join [{0}].[DataSource].[Institution] ins on pre.PreMas_InstitutionID = ins.Ins_ID
                            where 1 = 1 -- View_PreCooCli.CooCliType = -1 /* 合作跟非合作只差在這 */
                            and CAST(CashFlow_Time as Date) Between '{1}' and '{2}'
                            and CashFlow_Value <> 0 and CashFlow_IsEnable = 1) as c
                      PIVOT (
                    -- 設定彙總欄位及方式
                    Sum(CashFlow_Value)
                    -- 設定轉置欄位，並指定轉置欄位中需彙總的條件值作為新欄位
                    FOR CashFlow_Name IN ([部分負擔], [自費], [自費調劑],[押金], [其他]) ) cashflow) as Total
                    left join [{0}].[Customer].[Master] cus on Total.PreMas_CustomerID = cus.Cus_ID
                ", Properties.Settings.Default.SystemSerialNumber, sdate.ToString("yyyy-MM-dd"), edate.ToString("yyyy-MM-dd"));
            
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