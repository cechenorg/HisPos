using System;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.StoreOrder.Report;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.PurchaseReturnReport
{
    public class PurchaseReturnReportViewModel : TabBase
    {
        public override TabBase getTab()
        {
            return this;
        }

        #region ----- Define Commands -----
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand<string> ChangeTaxFlagCommand { get; set; }
        public RelayCommand ExportCSVCommand { get; set; }
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Variables /////
        private DateTime startDate;
        private DateTime endDate;
        private string manufactoryName;

        public DateTime StartDate
        {
            get { return startDate; }
            set { Set(() => StartDate, ref startDate, value); }
        }
        public DateTime EndDate
        {
            get { return endDate; }
            set { Set(() => EndDate, ref endDate, value); }
        }
        public string ManufactoryName
        {
            get { return manufactoryName; }
            set { Set(() => ManufactoryName, ref manufactoryName, value); }
        }
        #endregion

        private ManufactoryOrders manufactoryOrderCollection;
        private ManufactoryOrder currentManufactoryOrder;

        public ManufactoryOrders ManufactoryOrderCollection
        {
            get { return manufactoryOrderCollection; }
            set { Set(() => ManufactoryOrderCollection, ref manufactoryOrderCollection, value); }
        }
        public ManufactoryOrder CurrentManufactoryOrder
        {
            get { return currentManufactoryOrder; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetOrderDetails();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentManufactoryOrder, ref currentManufactoryOrder, value);
            }
        }
        #endregion

        public PurchaseReturnReportViewModel()
        {
            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void SearchAction()
        {
            if(!IsSearchConditionValid()) return;

            MainWindow.ServerConnection.OpenConnection();
            ManufactoryOrderCollection = ManufactoryOrders.GetManufactoryOrdersBySearchCondition(StartDate, EndDate, ManufactoryName);
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ChangeTaxFlagAction(string taxFlag)
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrentManufactoryOrder.ChangeIncludeTaxFlag(taxFlag.Equals("T"));
            MainWindow.ServerConnection.CloseConnection();
        }
        private void ExportCSVAction()
        {
            CurrentManufactoryOrder.ExportToCSV();
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ChangeTaxFlagCommand = new RelayCommand<string>(ChangeTaxFlagAction);
            ExportCSVCommand = new RelayCommand(ExportCSVAction);
        }
        private bool IsSearchConditionValid()
        {
            return false;
        }
        #endregion
    }
}
