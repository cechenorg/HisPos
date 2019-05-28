using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.WareHouse
{
   public class WareHouses : ObservableCollection<WareHouse>
    {
        private WareHouses(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new WareHouse(row));
            }
        }

        internal static WareHouses GetWareHouses()
        {
            return new WareHouses(WareHouseDb.Init());
        }
    }
}
