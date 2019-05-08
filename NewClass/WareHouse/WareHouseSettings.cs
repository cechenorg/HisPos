using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.WareHouse
{
    public class WareHouseSettings : Collection<WareHouseSetting>
    {
        public WareHouseSettings(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                Add(new WareHouseSetting(row));
            }
        }
    }
}
