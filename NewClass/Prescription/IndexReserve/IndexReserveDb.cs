using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.IndexReserve
{
   public static class IndexReserveDb
    {
        public static DataTable GetDataByDate(DateTime sDate, DateTime eDate)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "eDate", eDate);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[IndexReserveByDate]", parameterList);
        }
        public static DataTable Save(int Id,string PhoneCallStatus, bool IsNoPrepareMed)
        {
            string PrepareStatus = IsNoPrepareMed ? "F" : "D";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "ID", Id);
            DataBaseFunction.AddSqlParameter(parameterList, "PhoneCallStatus", PhoneCallStatus);
            DataBaseFunction.AddSqlParameter(parameterList, "PrepareStatus", PrepareStatus);
            DataBaseFunction.AddSqlParameter(parameterList, "userID", ChromeTabViewModel.ViewModelMainWindow.CurrentUser.ID);
            
            return MainWindow.ServerConnection.ExecuteProc("[Set].[UpdateIndexReserveStatus]", parameterList);
        } 
    }
}
