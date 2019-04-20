using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.Medicine.MedicineSet;
using StringRes = His_Pos.Properties.Resources;
using MedSelectWindow = His_Pos.FunctionWindow.AddProductWindow.AddMedicineWindow;

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
        public RelayCommand<string> AddMedicine { get; set; }
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
                CurrentSet.MedicineSetItems.GetItems(CurrentSet.ID);
            }
            InitialCommands();
        }

        private void InitialCommands()
        {
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            Save = new RelayCommand(SaveAction);
            Cancel = new RelayCommand(CancelAction);
        }
        #region CommandActions
        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MedicineSetItem m = new MedicineSetItem();
                switch (medicineID)
                {
                    case "R001":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "處方箋遺失或毀損，提前回診";
                        m.Amount = 0;
                        CurrentSet.MedicineSetItems.Add(m);
                        return;
                    case "R002":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "醫師請假，提前回診";
                        m.Amount = 0;
                        CurrentSet.MedicineSetItems.Add(m);
                        return;
                    case "R003":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "病情變化提前回診，經醫師認定需要改藥或調整藥品劑量或換藥";
                        m.Amount = 0;
                        CurrentSet.MedicineSetItems.Add(m);
                        return;
                    case "R004":
                        m.ID = medicineID;
                        m.NHIPrice = 0;
                        m.ChineseName = "其他提前回診或慢箋提前領藥";
                        m.Amount = 0;
                        CurrentSet.MedicineSetItems.Add(m);
                        return;
                    default:
                        MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "5", MessageType.WARNING);
                        return;
                }
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount == 0)
                MessageWindow.ShowMessage(StringRes.查無藥品, MessageType.WARNING);
            else
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                var MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.MedicineSetWindow);
                if (productCount > 1)
                    MedicineWindow.ShowDialog();
            }
        }
        private void DeleteMedicineAction()
        {
            CurrentSet.MedicineSetItems.Remove(CurrentSet.SelectedMedicine);
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
        private void GetSelectedProduct(NotificationMessage<ProductStruct> msg)
        {
            if (msg.Notification != nameof(MedicineSetViewModel)) return;
            Messenger.Default.Unregister<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
            CurrentSet.AddMedicine(msg.Content);
        }
        #endregion
        #region Functions
        private bool CheckSetValid()
        {
            if (CurrentSet.MedicineSetItems.Count == 0)
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
