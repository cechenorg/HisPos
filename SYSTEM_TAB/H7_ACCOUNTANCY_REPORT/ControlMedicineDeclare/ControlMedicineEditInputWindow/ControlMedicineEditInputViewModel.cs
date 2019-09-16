using GalaSoft.MvvmLight;
using His_Pos.NewClass.WareHouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditInputWindow
{
  public class ControlMedicineEditInputViewModel : ViewModelBase
    {
        private string medicineID;
        public string MedicineID
        {
            get { return medicineID; }
            set
            {
                Set(() => MedicineID,ref medicineID,value);
            }
        }
        private WareHouse selectedWareHouse;
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value);
            }
        }
        public WareHouses WareHouseCollection { get; set; } = WareHouses.GetWareHouses();
        public ControlMedicineEditInputViewModel() {
        }
    }
}
