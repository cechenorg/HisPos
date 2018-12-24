using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Position
{
    public static class PositionDb
    {
        public static ObservableCollection<Position> GetData()
        {
            var positions = new ObservableCollection<Position>();
            var dbConnection = new DatabaseConnection(Settings.Default.SQL_local);
            var divisionTable = dbConnection.ExecuteProc("[HIS_POS_DB].[PrescriptionDecView].[GetPositionData]");
            foreach (DataRow row in divisionTable.Rows)
            {
                var d = new Position(row);
                positions.Add(d);
            }
            return positions;
        }
    }
}
