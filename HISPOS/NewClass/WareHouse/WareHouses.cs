using System.Collections.ObjectModel;
using System.Data;

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