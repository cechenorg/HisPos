using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.Class.Product;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.Class.Leave
{
    public static class LeaveDb
    {
        internal static ObservableCollection<Leave> GetLeaveType()
        {
            ObservableCollection<Leave> collection = new ObservableCollection<Leave>();

            var dd = new DbConnection(Settings.Default.SQL_global);

            var table = dd.ExecuteProc("[HIS_POS_DB].[WorkScheduleManageView].[GetLeaveType]");

            foreach (DataRow row in table.Rows)
            {
                collection.Add( new Leave(row));
            }

            return collection;
        }
    }
}
