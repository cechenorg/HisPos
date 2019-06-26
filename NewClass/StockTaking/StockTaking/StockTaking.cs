using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.NewClass.StockTaking.StockTaking
{
    public class StockTaking : ObservableObject
    {
        public StockTaking(DataRow r) {
            ID = r.Field<string>("StoTakMas_ID");
            WareHouse = ChromeTabViewModel.ViewModelMainWindow.GetWareHouse(r.Field<int>("StoTakMas_WarehouseID").ToString());
            EnpName = r.Field<string>("Emp_Name");
            Time = r.Field<DateTime>("StoTakMas_Time");
        }
        public string ID { get; set; }
        public WareHouse.WareHouse WareHouse { get; set; }
        public string EnpName { get; set; }
        public DateTime Time { get; set; }
    }
}
