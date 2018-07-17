using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Division
{
    public static class DivisionDb
    {
        public static ObservableCollection<Division> GetData()
        {
            ObservableCollection<Division> divisions = new ObservableCollection<Division>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetDivisionsData]", dbConnection);
            foreach (DataRow division in divisionTable.Rows)
            {
                var d = new Division(division);
                divisions.Add(d);
            }
            return divisions;
        }
    }
}
