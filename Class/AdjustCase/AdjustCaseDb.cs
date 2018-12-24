﻿using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.AdjustCase
{
    public class AdjustCaseDb
    {
        internal static ObservableCollection<AdjustCase> GetData()
        {
            ObservableCollection<AdjustCase> adjustCases = new ObservableCollection<AdjustCase>();
            var dbConnection = new DatabaseConnection(Settings.Default.SQL_local);
            var divisionTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetAdjustCasesData]");
            foreach (DataRow adjustcase in divisionTable.Rows)
            {
                adjustCases.Add(new AdjustCase(adjustcase));
            }
            return adjustCases;
        }
    }
}