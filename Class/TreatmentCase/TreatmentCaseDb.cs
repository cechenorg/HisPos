using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.TreatmentCase
{
    public class TreatmentCaseDb : ISelection
    {
        public ObservableCollection<TreatmentCase> TreatmentCases { get;} = new ObservableCollection<TreatmentCase>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var prescriptionCaseTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetTreatmentCasesData]", dbConnection);
            foreach (DataRow treatmentCase in prescriptionCaseTable.Rows)
            {
                var t = new TreatmentCase(treatmentCase);
                TreatmentCases.Add(t);
            }
        }
        /*
         *回傳對應處方案件之id + name string
         */
        public string GetTreatmentCase(string tag)
        {
            GetData();
            var result = string.Empty;
            foreach (var treatment in TreatmentCases)
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
