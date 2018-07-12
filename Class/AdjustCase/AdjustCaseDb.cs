using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.AdjustCase
{
    public class AdjustCaseDb
    {
        public readonly ObservableCollection<AdjustCase> AdjustCases = new ObservableCollection<AdjustCase>();

        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetAdjustCasesData]", dbConnection);
            foreach (DataRow adjustcase in divisionTable.Rows)
            {
                AdjustCases.Add(new AdjustCase(adjustcase));
            }
        }
    }
}