using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using His_Pos.Interface;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Division
{
    public class DivisionDb : ISelection
    {
        public readonly ObservableCollection<Division> Divisions = new ObservableCollection<Division>();
        public void GetData()
        {
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var divisionTable = dbConnection.SetProcName("[HIS_POS_DB].[PrescriptionDecView].[GetDivisionsData]", dbConnection);
            foreach (DataRow division in divisionTable.Rows)
            {
                var d = new Division(division);
                Divisions.Add(d);
            }
        }
    }
}
