using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Medicine.MedicineSet;
using His_Pos.NewClass.Product;
using System.Data;
using System.Diagnostics;
using MedSelectWindow = His_Pos.FunctionWindow.AddProductWindow.AddMedicineWindow;
using StringRes = His_Pos.Properties.Resources;

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
        public RelayCommand Delete { get; set; }
        public RelayCommand Cancel { get; set; }

        #endregion Commands

        public MedicineSetViewModel(MedicineSetMode mode, MedicineSet selected = null)
        {
            Mode = mode;
            CurrentSet = new MedicineSet();
            if (Mode.Equals(MedicineSetMode.Edit))
            {
                Debug.Assert(selected != null, nameof(selected) + " != null");
                CurrentSet.ID = selected.ID;
                CurrentSet.Name = selected.Name;
                CurrentSet.MedicineSetItems.GetItems(CurrentSet.ID);
            }
            InitialCommands();
        }

        private void InitialCommands()
        {
            AddMedicine = new RelayCommand<string>(AddMedicineAction);
            DeleteMedicine = new RelayCommand(DeleteMedicineAction);
            Save = new RelayCommand(SaveAction);
            Delete = new RelayCommand(DeleteAction);
            Cancel = new RelayCommand(CancelAction);
        }

        #region CommandActions

        private void AddMedicineAction(string medicineID)
        {
            if (string.IsNullOrEmpty(medicineID)) return;
            if (medicineID.Length < 5)
            {
                MessageWindow.ShowMessage(StringRes.搜尋字串長度不足 + "5", MessageType.WARNING);
                return;
            }
            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(medicineID, AddProductEnum.PrescriptionDeclare, "0");
            MainWindow.ServerConnection.CloseConnection();
            if (productCount == 0)
                MessageWindow.ShowMessage(StringRes.查無藥品, MessageType.WARNING);
            else
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                var MedicineWindow = new MedSelectWindow(medicineID, AddProductEnum.MedicineSetWindow, "0");
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
            if (!CheckSetValid()) return;
            MainWindow.ServerConnection.OpenConnection();
            var table = MedicineSetDb.UpdateMedicineSet(CurrentSet);
            if (table.Rows.Count == 1)
            {
                CurrentSet.ID = table.Rows[0].Field<int>("NewID");
                MessageWindow.ShowMessage("儲存成功", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseMedicineSetWindow"));
            }
            else
            {
                MessageWindow.ShowMessage("儲存異常，請稍後重試", MessageType.WARNING);
            }
            MainWindow.ServerConnection.CloseConnection();
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

        private void DeleteAction()
        {
            var deleteConfirm = new ConfirmWindow("確認刪除藥品組合:" + CurrentSet.Name, "");
            Debug.Assert(deleteConfirm.DialogResult != null, "deleteConfirm.DialogResult != null");
            if ((bool)deleteConfirm.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();
                MedicineSetDb.DeleteMedicineSet(CurrentSet.ID);
                MainWindow.ServerConnection.CloseConnection();
                MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseMedicineSetWindow"));
            }
        }

        #endregion CommandActions

        #region Functions

        private bool CheckSetValid()
        {
            if (CurrentSet.MedicineSetItems.Count == 0)
            {
                MessageWindow.ShowMessage("藥品組合不得為空", MessageType.ERROR);
                return false;
            }
            if (string.IsNullOrEmpty(CurrentSet.Name))
            {
                MessageWindow.ShowMessage("請輸入組合名稱", MessageType.ERROR);
                return false;
            }
            return true;
        }

        #endregion Functions
    }
}