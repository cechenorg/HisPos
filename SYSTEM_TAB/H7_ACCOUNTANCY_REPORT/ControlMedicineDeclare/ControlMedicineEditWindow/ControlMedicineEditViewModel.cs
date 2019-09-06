using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Manufactory;
using His_Pos.NewClass.Medicine.ControlMedicineEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #region Command
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        #endregion
        public ControlMedicineEditViewModel(string medID,string warID) {
            ControlMedicineEditCollection = ControlMedicineEdits.GetData(medID,warID);
            ManufactoryCollection = Manufactories.GetControlMedicineManufactories();
            TypeList = new List<string>() { "進貨","退貨"};
            for (int i = 0; i < ControlMedicineEditCollection.Count; i++) {
                ControlMedicineEditCollection[i].Manufactory = ManufactoryCollection.Single(m => m.ID == ControlMedicineEditCollection[i].ManufactoryID.ToString()); 
            }
            AddRowAction();
            DeleteCommand = new RelayCommand(DeleteAction);
            AddRowCommand = new RelayCommand(AddRowAction);
        }
        #region Action
        private void AddRowAction()
        {
            if (!(ControlMedicineEditSelectedItem is null) && CheckData())
                ControlMedicineEditSelectedItem.IsNew = false;
            else
                return;
             
            ControlMedicineEditCollection.Add(new ControlMedicineEdit() { IsNew = true});
            ControlMedicineEditSelectedItem = ControlMedicineEditCollection[ControlMedicineEditCollection.Count-1];
        }
        private void DeleteAction() {
            if (ControlMedicineEditSelectedItem is null) return;
            ControlMedicineEditCollection.Remove(ControlMedicineEditSelectedItem); 
        }
        private bool CheckData() {
            if (ControlMedicineEditSelectedItem.Amount == 0) {
                MessageWindow.ShowMessage("數量不可為0", Class.MessageType.ERROR);
                return false;
            }
               
        }
        #endregion
    }
}
