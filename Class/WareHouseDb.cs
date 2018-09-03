using His_Pos.Properties;
using His_Pos.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.Class
{
   public static class WareHouseDb
    {
        internal static ObservableCollection<WareHouse> GetWareHouseData()
        {
            var dd = new DbConnection(Settings.Default.SQL_global);
            var table = dd.ExecuteProc("[HIS_POS_DB].[InventoryManagementView].[GetWareHouseData]");
            ObservableCollection<WareHouse> data = new ObservableCollection<WareHouse>();
            
            foreach (DataRow row in table.Rows)
            {
                data.Add(new WareHouse(row));
            }
            return data;
        }
    }
}
