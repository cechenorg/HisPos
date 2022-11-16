using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using His_Pos.Database;
using His_Pos.NewClass.Report.IncomeStatement;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.NewClass.Report.StockTakingDetailReport.StockTakingDetailRecordReport;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace His_Pos.NewClass.Report
{
    public class ReportService
    {

        public static DataSet TodayCashStockEntryReport(string schema, DateTime startDAte, DateTime endDate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDAte);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            var result = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[TodayCashStockEntryReport]", parameterList, schema: schema);
            MainWindow.ServerConnection.CloseConnection();

            return result;
        }

        public static DataTable TradeRecordQuery(string MasID)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("MasterID", MasID));
            parameters.Add(new SqlParameter("CustomerID", DBNull.Value));
            parameters.Add(new SqlParameter("sDate", ""));
            parameters.Add(new SqlParameter("eDate", ""));
            parameters.Add(new SqlParameter("sInvoice", ""));
            parameters.Add(new SqlParameter("eInvoice", ""));
            parameters.Add(new SqlParameter("flag", "1"));
            parameters.Add(new SqlParameter("ShowIrregular", DBNull.Value));
            parameters.Add(new SqlParameter("ShowReturn", DBNull.Value));
            parameters.Add(new SqlParameter("Cashier", -1));
            parameters.Add(new SqlParameter("ProID", DBNull.Value));
            parameters.Add(new SqlParameter("ProName", DBNull.Value));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            return result;
        }

        public static DataTable GetStockTakingDetailRecordDataTableByDate(string Id, DateTime sDate, DateTime eDate)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Id", Id));
            parameters.Add(new SqlParameter("sDate", sDate));
            parameters.Add(new SqlParameter("eDate", eDate));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[POS].[TradeRecordQuery]", parameters);
            MainWindow.ServerConnection.CloseConnection();

            return result;
        }

        public static IEnumerable<StockTakingDetailRecordReport> GetStockTakingDetailRecordByDate(string Id, DateTime sDate, DateTime eDate)
        {
            IEnumerable<StockTakingDetailRecordReport> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<StockTakingDetailRecordReport>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[StockTakingDetailRecordByDate]",
                    param: new { Id = Id, sDate = sDate, eDate = eDate },
                    commandType: CommandType.StoredProcedure);

            });

            return result;
        }

        public static IEnumerable<PrescriptionDetailMedicineRepot> GetPrescriptionDetailMedicineReportById(int id, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<PrescriptionDetailMedicineRepot> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<PrescriptionDetailMedicineRepot>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[PrescriptionDetailMedicineReportById]",
                    param: new
                    {
                        Id = id,
                        sDate = startDate,
                        eDate = endDate
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }

        public static IEnumerable<IncomeStatementRawData> GetIncomeStatementRawData(int year, string accID = null)
        {
            IEnumerable<IncomeStatementRawData> result = default;
            SQLServerConnection.DapperQuery((conn) =>
            {
                result = conn.Query<IncomeStatementRawData>($"{Properties.Settings.Default.SystemSerialNumber}.[Get].[ISExpenses]",
                param: new
                    {
                        YEAR = year,
                        AccID = accID
                    },
                    commandType: CommandType.StoredProcedure);

            });
            return result;
        }

    }
}
