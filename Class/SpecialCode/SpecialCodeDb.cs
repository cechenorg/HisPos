using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.SpecialCode
{
    class SpecialCodeDb
    {
        public static ObservableCollection<SpecialCode> GetData()
        {
            ObservableCollection<SpecialCode> divisions = new ObservableCollection<SpecialCode>();
            var dbConnection = new DbConnection(Settings.Default.SQL_global);
            var specialCodes = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetSpecialTreatment]");
            foreach (DataRow specialCode in specialCodes.Rows)
            {
                var d = new SpecialCode(specialCode);
                divisions.Add(d);
            }
            return divisions;
        }
    }
}
