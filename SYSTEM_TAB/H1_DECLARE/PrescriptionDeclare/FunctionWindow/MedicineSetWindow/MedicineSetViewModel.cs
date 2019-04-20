using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.Medicine.MedicineSet;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.MedicineSetWindow
{
    public class MedicineSetViewModel : ViewModelBase
    {
        public MedicineSetMode Mode;
        private MedicineSet currentSet;
        public MedicineSet CurrentSet
        {
            get => currentSet;
            set
            {
                Set(() => CurrentSet, ref currentSet, value);
            }
        }

        #region Commands
        public RelayCommand DeleteMedicine { get; set; }
        public RelayCommand Save { get; set; }
        public RelayCommand Cancel { get; set; }
        #endregion
        public MedicineSetViewModel(MedicineSetMode mode, int? setID = null)
        {
            Mode = mode;
            CurrentSet = new MedicineSet();
            if (Mode.Equals(MedicineSetMode.Edit))
            {
                Debug.Assert(setID != null, nameof(setID) + " != null");
                CurrentSet.ID = (int)setID;
                CurrentSet.GetSetItems();
            }
            InitialCommands();
        }

        private void InitialCommands()
        {
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            Save = new RelayCommand(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }
        #region CommandActions
        private void DeleteMedicineAction()
        {
            CurrentSet.Medicines.Remove(CurrentSet.SelectedMedicine);
        }
        private void SaveAction()
        {
            if(!CheckSetValid()) return;
            MainWindow.ServerConnection.OpenConnection();
            MedicineSetDb.UpdateMedicineSet(CurrentSet);
            MainWindow.ServerConnection.CloseConnection();
            Messenger.Default.Send(new NotificationMessage("CloseMedicineSetWindow"));
        }
        private void CancelAction()
        {
            Messenger.Default.Send(new NotificationMessage("CloseMedicineSetWindow"));
        }
        #endregion
        #region Functions
        private bool CheckSetValid()
        {
            if (CurrentSet.Medicines.Count == 0)
            {
                MessageWindow.ShowMessage("藥品組合不得為空",MessageType.ERROR);
                return false;
            }
            if (string.IsNullOrEmpty(CurrentSet.Name))
            {
                MessageWindow.ShowMessage("請輸入組合名稱", MessageType.ERROR);
                return false;
            }
            return true;
        }
        #endregion
    }
}
