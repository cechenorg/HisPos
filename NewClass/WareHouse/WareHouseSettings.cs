using System.Collections.ObjectModel;
using System.Data;

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