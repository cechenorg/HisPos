using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.WareHouse;

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
                Set(() => MedicineID, ref medicineID, value);
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
        public RelayCommand SubmitCommand { get; set; }

        public ControlMedicineEditInputViewModel()
        {
            SubmitCommand = new RelayCommand(SubmitAction);
        }

        private void SubmitAction()
        {
            ControlMedicineEditWindow.ControlMedicineEditWindow controlMedicineEditWindow = new ControlMedicineEditWindow.ControlMedicineEditWindow(MedicineID, SelectedWareHouse.ID);
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseControlMedicineEditInputWindow"));
        }
    }
}