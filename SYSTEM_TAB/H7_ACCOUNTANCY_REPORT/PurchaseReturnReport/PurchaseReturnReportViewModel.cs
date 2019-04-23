using System;
using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.ChromeTabViewModel;
using His_Pos.Class;
using His_Pos.FunctionWindow;
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
        public RelayCommand ExportCSVCommand { get; set; }
        #endregion

        #region ----- Define Variables -----

        #region ///// Search Variables /////
        private DateTime? startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private DateTime? endDate = DateTime.Today;
        private string manufactoryName = "";

        public DateTime? StartDate
        {
            get { return startDate; }
            set { Set(() => StartDate, ref startDate, value); }
        }
        public DateTime? EndDate
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

        private DateTime SearchStartDate { get; set; }
        private DateTime SearchEndDate { get; set; }
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
                value?.GetOrderDetails(SearchStartDate, SearchEndDate);
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrentManufactoryOrder, ref currentManufactoryOrder, value);

                RaisePropertyChanged("HasManufactory");
            }
        }
        public bool HasManufactory { get { return CurrentManufactoryOrder != null; } }
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

            SearchStartDate = (DateTime)StartDate;
            SearchEndDate = (DateTime)EndDate;
        }
        private void ExportCSVAction()
        {
            CurrentManufactoryOrder.ExportToCSV(SearchStartDate, SearchEndDate);
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            SearchCommand = new RelayCommand(SearchAction);
            ExportCSVCommand = new RelayCommand(ExportCSVAction);
        }
        private bool IsSearchConditionValid()
        {
            if (StartDate is null || EndDate is null)
            {
                MessageWindow.ShowMessage("查詢時間為必填欄位!", MessageType.ERROR);
                return false;
            }

            if (EndDate < StartDate)
            {
                MessageWindow.ShowMessage("起始日期大於終結日期!", MessageType.ERROR);
                return false;
            }
            
            return true;
        }
        #endregion
    }
}
