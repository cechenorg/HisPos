using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.TreatmentCase
{
    public static class TreatmentCaseDb
    {
        public static ObservableCollection<TreatmentCase> GetData()
        {
            ObservableCollection<TreatmentCase> treatmentCases = new ObservableCollection<TreatmentCase>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var prescriptionCaseTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetTreatmentCasesData]");
            foreach (DataRow treatmentCase in prescriptionCaseTable.Rows)
            {
                var t = new TreatmentCase(treatmentCase);
                treatmentCases.Add(t);
            }

            return treatmentCases;
        }
        /*
         *回傳對應處方案件之id + name string
         */
        public static string GetTreatmentCase(string tag)
        {
            GetData();
            var result = string.Empty;
            foreach (var treatment in GetData())
            {
                if (treatment.Id == tag)
                {
                    result = treatment.FullName;
                }
            }
            return result;
        }
    }
}
