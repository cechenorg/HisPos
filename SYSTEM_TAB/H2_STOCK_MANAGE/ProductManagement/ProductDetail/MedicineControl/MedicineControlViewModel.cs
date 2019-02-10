using System.Data;
using GalaSoft.MvvmLight.Command;
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
        #endregion

        #region ----- Define Variables -----
        public ProductManageMedicine Medicine { get; set; }
        public ProductManageMedicine BackUpMedicine { get; set; }
        public ProductManageMedicineDetail MedicineDetail { get; set; }
        #endregion
        
        public MedicineControlViewModel()
        {
        }

        public MedicineControlViewModel(string id)
        {
            RegisterCommand();
            InitMedicineData(id);
        }
        
        #region ----- Define Actions -----
        private void ConfirmChangeAction()
        {

        }
        private void CancelChangeAction()
        {

        }
        private void SyncDataAction()
        {

        }
        private void StockTakingAction()
        {

        }
        private void ViewHistoryPriceAction()
        {

        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            ConfirmChangeCommand = new RelayCommand(ConfirmChangeAction);
            CancelChangeCommand = new RelayCommand(CancelChangeAction);
            SyncDataCommand = new RelayCommand(SyncDataAction);
            StockTakingCommand = new RelayCommand(StockTakingAction);
            ViewHistoryPriceCommand = new RelayCommand(ViewHistoryPriceAction);
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
        #endregion
    }
}
