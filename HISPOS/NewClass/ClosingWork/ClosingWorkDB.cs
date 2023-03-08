using His_Pos.ChromeTabViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.ClosingWork
{
    public class ClosingWorkDB
    {
        public static DataTable InsertCloseCash(int total, int checkTotal, DateTime date)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("EMP", ViewModelMainWindow.CurrentUser.ID));
            parameters.Add(new SqlParameter("Value", total - checkTotal));
            parameters.Add(new SqlParameter("Total", checkTotal));
            parameters.Add(new SqlParameter("Close_Date", date));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[InsertCloseCash]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public static DataTable GetClosingWorkToJournal(DateTime date)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("Date", date));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkToJournal]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public static DataTable SetClosingWorkToJournal(DateTime date)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@cDate", date));
            parameters.Add(new SqlParameter("@cEmpID", ViewModelMainWindow.CurrentUser.ID));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Set].[ClosingWorkToJournal]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }

        public static DataTable ClosingWorkByDate(DateTime date)
        {
            MainWindow.ServerConnection.OpenConnection();
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("sDate", date));
            parameters.Add(new SqlParameter("eDate", date));
            DataTable result = MainWindow.ServerConnection.ExecuteProc("[Get].[ClosingWorkByDate]", parameters);
            MainWindow.ServerConnection.CloseConnection();
            return result;
        }
    }
}
