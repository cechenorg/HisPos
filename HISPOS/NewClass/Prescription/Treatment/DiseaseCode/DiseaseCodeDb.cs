using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode
{
    public static class DiseaseCodeDb
    {
        public static DataTable GetDataByCodeId(string code)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Code", code));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[DiseaseCodeByCodeId]", parameterList);
            return table;
        }

        public static DataTable GetDiseaseCodes()
        {
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[OffLineDiseaseCode]");
            return table;
        }

        public static DataTable GetICD9DiseaseCodes()
        {
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[OffLineDiseaseCodeMapping]");
            return table;
        }
    }
}