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
        public WareHouses() { }

        public void Init() {
            DataTable table = WareHouseDb.Init();
            foreach (DataRow r in table.Rows) {
                Add(new WareHouse(r));
            }
        }
    }
}
