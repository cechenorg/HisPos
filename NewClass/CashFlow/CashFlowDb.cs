using His_Pos.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
namespace His_Pos.NewClass.CashFlow {
    public static class CashFlowDb {
        public static DataTable GetCashFlowByDate(DateTime sDate) {
            string CooperativeInstitutionID = WebApi.GetCooperativeClinicId(VM.CurrentPharmacy.ID);
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "sDate", sDate);
            DataBaseFunction.AddSqlParameter(parameterList, "insId", CooperativeInstitutionID);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[CashFlowByDate]", parameterList);
        }
           
    }
}
