using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.Medicine.ControlMedicineDeclare;
using His_Pos.NewClass.Product.Medicine.ControlMedicineDetail;
using System;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare {
    public class ControlMedicineDeclareViewModel : TabBase {
        public override TabBase getTab() {
            return this;
        }

        #region Var
        private DateTime sDateTime = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        public DateTime SDateTime
        {
            get { return sDateTime; }
            set
            {
                Set(() => SDateTime, ref sDateTime, value);
            }
        }
        private DateTime eDateTime = DateTime.Now;
        public DateTime EDateTime
        {
            get { return eDateTime; }
            set
            {
                Set(() => EDateTime, ref eDateTime, value);
            }
        }
        private ControlMedicineDeclares controlMedicineDeclares = new ControlMedicineDeclares();
        public ControlMedicineDeclares ControlMedicineDeclares
        {
            get { return controlMedicineDeclares; }
            set
            {
                Set(() => ControlMedicineDeclares, ref controlMedicineDeclares, value);
            }
        }
        private NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare selectItem;
        public NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare SelectItem
        {
            get { return selectItem; }
            set
            {
                Set(() => SelectItem, ref selectItem, value);
            }
        }
        private ControlMedicineDetails controlMedicineDetailsCollection = new ControlMedicineDetails();
        public ControlMedicineDetails ControlMedicineDetailsCollection
        {
            get { return controlMedicineDetailsCollection; }
            set
            {
                Set(() => ControlMedicineDetailsCollection, ref controlMedicineDetailsCollection, value);
            }
        }
        private ControlMedicineDetails controlMedicineBagDetailsCollection = new ControlMedicineDetails();
        public ControlMedicineDetails ControlMedicineBagDetailsCollection
        {
            get { return controlMedicineBagDetailsCollection; }
            set
            {
                Set(() => ControlMedicineBagDetailsCollection, ref controlMedicineBagDetailsCollection, value);
            }
        }
        #endregion
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public ControlMedicineDeclareViewModel() {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
        }
        private void SearchAction() {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
        }
        private void SelectionChangedAction() {
            if (SelectItem == null) return;
            ControlMedicineDetails temp = new ControlMedicineDetails();
            temp.GetDataById(SelectItem.ID, SDateTime, EDateTime);
            ControlMedicineBagDetailsCollection.Clear();
            ControlMedicineDetailsCollection.Clear();
            foreach (ControlMedicineDetail c in temp) {
                switch (c.TypeName) {
                    case "處方調劑(未過卡)":
                        ControlMedicineBagDetailsCollection.Add(c);
                        break;
                    default:
                        ControlMedicineDetailsCollection.Add(c);
                        break;
                }  
            } 
        }
    }
}
