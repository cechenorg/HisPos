using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Medicine.ControlMedicineEdit;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow
{
    public class ControlMedicineEditViewModel : ViewModelBase
    {
        private ControlMedicineEdits controlMedicineEditCollection;

        public ControlMedicineEdits ControlMedicineEditCollection
        {
            get { return controlMedicineEditCollection; }
            set
            {
                Set(() => ControlMedicineEditCollection, ref controlMedicineEditCollection, value);
            }
        }

        private ControlMedicineEdit controlMedicineEditSelectedItem;

        public ControlMedicineEdit ControlMedicineEditSelectedItem
        {
            get { return controlMedicineEditSelectedItem; }
            set
            {
                Set(() => ControlMedicineEditSelectedItem, ref controlMedicineEditSelectedItem, value);
                for (int i = 0; i < ControlMedicineEditCollection.Count; i++)
                {
                    ControlMedicineEditCollection[i].IsSelect = false;
                }
                if (value is null) return;
                value.IsSelect = true;
            }
        }

        private List<string> typeList;

        public List<string> TypeList
        {
            get { return typeList; }
            set
            {
                Set(() => TypeList, ref typeList, value);
            }
        }

        private Manufactories manufactoryCollection;

        public Manufactories ManufactoryCollection
        {
            get { return manufactoryCollection; }
            set
            {
                Set(() => ManufactoryCollection, ref manufactoryCollection, value);
            }
        }

        public string MedicineID { get; set; }
        public string WarID { get; set; }

        #region Command

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        public RelayCommand UpdateCommand { get; set; }

        #endregion Command

        public ControlMedicineEditViewModel(string medID, string warID)
        {
            MedicineID = medID;
            WarID = warID;
            ControlMedicineEditCollection = ControlMedicineEdits.GetData(MedicineID, WarID);
             
            DataTable table = ManufactoryDB.GetAllManufactories();
            Manufactories manufactories = new Manufactories();
            foreach (DataRow row in table.Rows)
            {
                ControlManufactory manufactory = new ControlManufactory(row);
                if (!string.IsNullOrEmpty(manufactory.ControlmedicineID))
                    manufactories.Add(manufactory);
            }

            ManufactoryCollection = manufactories;

            TypeList = new List<string>() { "進貨", "報廢" };
            for (int i = 0; i < ControlMedicineEditCollection.Count; i++)
            {
                if (ManufactoryCollection.Count(m => m.ID == ControlMedicineEditCollection[i].ManufactoryID.ToString()) == 1)
                    ControlMedicineEditCollection[i].Manufactory = ManufactoryCollection.Single(m => m.ID == ControlMedicineEditCollection[i].ManufactoryID.ToString());
            }
            AddRowAction();
            DeleteCommand = new RelayCommand(DeleteAction);
            AddRowCommand = new RelayCommand(AddRowAction);
            UpdateCommand = new RelayCommand(UpdateAction);
        }

        #region Action

        private void UpdateAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否更新?", "管制藥品更新確認");
            if ((bool)confirmWindow.DialogResult)
            {
                ControlMedicineEditCollection.Update(MedicineID, WarID);
                MessageWindow.ShowMessage("更新成功", NewClass.MessageType.SUCCESS);
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("ControlMedicineDeclareSearch"));
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("CloseControlMedicineEditWindow"));
            }
        }

        private void AddRowAction()
        {
            if (!(ControlMedicineEditSelectedItem is null))
            {
                if (CheckData())
                    ControlMedicineEditSelectedItem.IsNew = false;
                else
                    return;
            }

            ControlMedicineEditCollection.Add(new ControlMedicineEdit(MedicineID, WarID) { IsNew = true, Type = "進貨" });
            ControlMedicineEditSelectedItem = ControlMedicineEditCollection[ControlMedicineEditCollection.Count - 1];
        }

        private void DeleteAction()
        {
            if (ControlMedicineEditSelectedItem is null) return;
            ControlMedicineEditCollection.Remove(ControlMedicineEditSelectedItem);
        }

        private bool CheckData()
        {
            if (ControlMedicineEditSelectedItem.Amount <= 0)
            {
                MessageWindow.ShowMessage("數量不可小於等於0", NewClass.MessageType.ERROR);
                return false;
            }
            if (ControlMedicineEditSelectedItem.Manufactory is null && ControlMedicineEditSelectedItem.Type == "進貨")
            {
                MessageWindow.ShowMessage("請選擇供應商", NewClass.MessageType.ERROR);
                return false;
            }
            if (string.IsNullOrEmpty(ControlMedicineEditSelectedItem.Type))
            {
                MessageWindow.ShowMessage("請選擇類別", NewClass.MessageType.ERROR);
                return false;
            }
            return true;
        }

        #endregion Action
    }
}