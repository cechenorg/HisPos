using His_Pos.Database;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot
{
    public static class PrescriptionDetailMedicineRepotDb
    {
        public static DataTable GetDataById(int Id)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            DataBaseFunction.AddSqlParameter(parameterList, "Id", Id);
            return MainWindow.ServerConnection.ExecuteProc("[Get].[PrescriptionDetailMedicineReportById]", parameterList);
        }
    }
}