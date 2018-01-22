using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class
{
    public class TreatmentCases : ISelection
    {
        public List<TreatmentCase> TreatmentCaseLsit { get; } = new List<TreatmentCase>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var prescriptionCaseTable = dbConnection.SetProcName("[HIS_POS_DB].[GET].[MEDICALCASE]", dbConnection);
            foreach (DataRow treatmentCase in prescriptionCaseTable.Rows)
            {
                var t = new TreatmentCase(treatmentCase["HISMEDCAS_ID"].ToString(), treatmentCase["HISMEDCAS_NAME"].ToString());
                TreatmentCaseLsit.Add(t);
            }
        }
    }
}
