using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.Prescription.Treatment.DiseaseCode {
    public static class DiseaseCodeDb {
        public static DataTable GetDataByCodeId(string code) {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Code", code));
            var table = MainWindow.ServerConnection.ExecuteProc("[Get].[DiseaseCodeByCodeId]", parameterList);
            return table;  
        }

    }
}
