using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StoreOrder;

namespace His_Pos.NewClass.Medicine.InventoryMedicineStruct
{
    public class NotEnoughMedicineStructs: Collection<NotEnoughMedicineStruct>
    {
        public string StoreOrderID { get; set; }

        public NotEnoughMedicineStructs()
        {
            MainWindow.ServerConnection.OpenConnection();
            var count = StoreOrderDB.GetStoOrdMasterCountByDate().Rows[0].Field<int>("Count");
            var newStoOrdID = "P" + DateTime.Today.ToString("yyyyMMdd") + "-" + count.ToString().PadLeft(2, '0');
            StoreOrderID = newStoOrdID;
            MainWindow.ServerConnection.CloseConnection();
        }
    }
}
