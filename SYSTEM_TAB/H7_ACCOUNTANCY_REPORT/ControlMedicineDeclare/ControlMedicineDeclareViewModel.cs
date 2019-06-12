using GalaSoft.MvvmLight.Command;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product.Medicine.ControlMedicineDeclare;
using His_Pos.NewClass.Product.Medicine.ControlMedicineDetail;
using His_Pos.NewClass.WareHouse;
using System;
using System.ComponentModel;
using System.Windows.Data;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare
{
    public class ControlMedicineDeclareViewModel : TabBase
    {
        public override TabBase getTab()
        {
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
        private CollectionViewSource controlCollectionViewSource;
        private CollectionViewSource ControlCollectionViewSource
        {
            get => controlCollectionViewSource;
            set
            {
                Set(() => ControlCollectionViewSource, ref controlCollectionViewSource, value); 
            }
        }

        private ICollectionView controlCollectionView;
        public ICollectionView ControlCollectionView
        {
            get => controlCollectionView;
            private set
            {
                Set(() => ControlCollectionView, ref controlCollectionView, value);
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
        private NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare  selectItem;
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
        #endregion
        public RelayCommand SelectionChangedCommand { get; set; }
        public RelayCommand SearchCommand { get; set; } 
        public RelayCommand WareHouseSelectionChangedCommand { get; set; }
        public ControlMedicineDeclareViewModel()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            SelectionChangedCommand = new RelayCommand(SelectionChangedAction);
            SearchCommand = new RelayCommand(SearchAction);
            WareHouseSelectionChangedCommand = new RelayCommand(WareHouseSelectionChangedAction);
            SelectedWareHouse = WareHouseCollection[0];
            SearchAction();
        }
        private void WareHouseSelectionChangedAction() {
            ControlCollectionViewSource.Filter += Filter;
        }
        private void SearchAction()
        {
            ControlMedicineDeclares.GetData(SDateTime, EDateTime);
            ControlCollectionViewSource = new CollectionViewSource { Source = ControlMedicineDeclares };
            ControlCollectionView = ControlCollectionViewSource.View;
            ControlCollectionViewSource.Filter += Filter;
        }
        private void SelectionChangedAction()
        {
            if (SelectItem == null) return;
            ControlMedicineDetails temp = new ControlMedicineDetails();
            temp.GetDataById(SelectItem.ID, SDateTime, EDateTime, SelectItem.InitStock,SelectedWareHouse.ID);
            ControlMedicineBagDetailsCollection.Clear();
            ControlMedicineDetailsCollection.Clear();
            foreach (ControlMedicineDetail c in temp)
            {
                switch (c.TypeName)
                {
                    case "調劑(未過卡)":
                        ControlMedicineBagDetailsCollection.Add(c);
                        break;
                    default:
                        ControlMedicineDetailsCollection.Add(c);
                        break;
                }
            }
        }
        private void Filter(object sender, FilterEventArgs e) {
            if (e.Item is null) return;
            if (!(e.Item is NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare src))
                e.Accepted = false;
            NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare controlMedicineDeclare = ((NewClass.Product.Medicine.ControlMedicineDeclare.ControlMedicineDeclare)e.Item);
            e.Accepted = controlMedicineDeclare.WareHouse.ID == SelectedWareHouse.ID; 
        }
    }
}
