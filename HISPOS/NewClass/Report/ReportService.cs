using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace His_Pos.NewClass.Report
{
    public class ReportService
    {

        public static DataSet TodayCashStockEntryReport(DateTime startDAte, DateTime endDate)
        {

            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", startDAte);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", endDate);
            var result = MainWindow.ServerConnection.ExecuteProcReturnDataSet("[Get].[TodayCashStockEntryReport]", parameterList);
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
    }
}
