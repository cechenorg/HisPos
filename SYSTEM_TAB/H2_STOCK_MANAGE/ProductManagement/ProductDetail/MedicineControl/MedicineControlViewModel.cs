using System.Data;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    class MedicineControlViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand ConfirmChangeCommand { get; set; }
        public RelayCommand CancelChangeCommand { get; set; }
        public RelayCommand SyncDataCommand { get; set; }
        public RelayCommand StockTakingCommand { get; set; }
        public RelayCommand ViewHistoryPriceCommand { get; set; }
        public RelayCommand DataChangedCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isDataChanged;
        private string newInventory = "";
        private ProductManageMedicine medicine;

        public bool IsDataChanged
        {
            get { return isDataChanged; }
            set
            {
                Set(() => IsDataChanged, ref isDataChanged, value);
                CancelChangeCommand.RaiseCanExecuteChanged();
                ConfirmChangeCommand.RaiseCanExecuteChanged();
            }
        }
        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                Set(() => NewInventory, ref newInventory, value);
                StockTakingCommand.RaiseCanExecuteChanged();
            }
        }
        public ProductManageMedicine Medicine
        {
            get { return medicine; }
            set
            {
                Set(() => Medicine, ref medicine, value);
            }
        }
        public ProductManageMedicine BackUpMedicine { get; set; }
        public ProductManageMedicineDetail MedicineDetail { get; set; }
        #endregion
        
        public MedicineControlViewModel(string id)
        {
            RegisterCommand();
            InitMedicineData(id);
        }
        
        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {
            IsDataChanged = false;
        }
        private void CancelChangeAction()
        {
            Medicine = BackUpMedicine.Clone() as ProductManageMedicine;

            IsDataChanged = false;
        }
        private void SyncDataAction()
        {
            MainWindow.ServerConnection.OpenConnection();
            //ProductDetailDB.GetProductManageMedicineDataByID(id);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID);
        }
        private void StockTakingAction()
        {

        }
        private void ViewHistoryPriceAction()
        {

        }
        private void DataChangedAction()
        {
            IsDataChanged = true;
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction, IsMedicineDataChanged);
            CancelChangeCommand = new RelayCommand(CancelChangeAction, IsMedicineDataChanged);
            SyncDataCommand = new RelayCommand(SyncDataAction);
            StockTakingCommand = new RelayCommand(StockTakingAction, IsNewInventoryHasValue);
            ViewHistoryPriceCommand = new RelayCommand(ViewHistoryPriceAction);
            DataChangedCommand = new RelayCommand(DataChangedAction);
        }
        private void InitMedicineData(string id)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable manageMedicineDataTable = ProductDetailDB.GetProductManageMedicineDataByID(id);
            MainWindow.ServerConnection.CloseConnection();

            if (manageMedicineDataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            Medicine = new ProductManageMedicine(manageMedicineDataTable.Rows[0]);
            MedicineDetail = new ProductManageMedicineDetail(manageMedicineDataTable.Rows[0]);

            BackUpMedicine = Medicine.Clone() as ProductManageMedicine;
        }
        private bool IsMedicineDataChanged()
        {
            return IsDataChanged;
        }
        private bool IsNewInventoryHasValue()
        {
            return !NewInventory.Equals(string.Empty);
        }
        #endregion
    }
}
