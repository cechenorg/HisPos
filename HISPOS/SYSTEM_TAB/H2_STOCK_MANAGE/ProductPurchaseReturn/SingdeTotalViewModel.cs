using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.StoreOrder.SingdeTotalOrder;
using System.ComponentModel;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class SingdeTotalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand<string> ToDoneCommand { get; set; }
        public RelayCommand AllToDoneCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private SingdeTotalOrder currenTotalOrder;
        private bool isBusy;
        private string busyContent;

        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }

        public string BusyContent
        {
            get => busyContent;
            set { Set(() => BusyContent, ref busyContent, value); }
        }

        public SingdeTotalOrder CurrenTotalOrder
        {
            get { return currenTotalOrder; }
            set
            {
                MainWindow.ServerConnection.OpenConnection();
                value?.GetProcessingOrders();
                MainWindow.ServerConnection.CloseConnection();
                Set(() => CurrenTotalOrder, ref currenTotalOrder, value);
            }
        }

        public SingdeTotalOrders TotalOrders { get; set; }
        public bool HasOrder { get { return CurrenTotalOrder != null; } }

        #endregion ----- Define Variables -----

        public SingdeTotalViewModel()
        {
            InitCommands();
        }

        #region ----- Define Actions -----

        private void ToDoneAction(string id)
        {
            MainWindow.ServerConnection.OpenConnection();
            CurrenTotalOrder.OrderToDone(id);
            MainWindow.ServerConnection.CloseConnection();
        }

        private void AllToDoneAction()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker();

            IsBusy = true;

            backgroundWorker.DoWork += (sender, args) =>
            {
                BusyContent = "處理中...";
                MainWindow.ServerConnection.OpenConnection();
                CurrenTotalOrder.AllOrderToDone();
                MainWindow.ServerConnection.CloseConnection();
            };

            backgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                IsBusy = false;
            };

            backgroundWorker.RunWorkerAsync();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        public void InitCommands()
        {
            ToDoneCommand = new RelayCommand<string>(ToDoneAction);
            AllToDoneCommand = new RelayCommand(AllToDoneAction);
        }

        internal void InitData()
        {
            MainWindow.ServerConnection.OpenConnection();
            TotalOrders = SingdeTotalOrders.GetSingdeTotalOrders();
            MainWindow.ServerConnection.CloseConnection();

            if (TotalOrders.Count > 0)
            {
                CurrenTotalOrder = TotalOrders[0];
            }
            else
                CurrenTotalOrder = null;

            RaisePropertyChanged(nameof(HasOrder));
        }

        #endregion ----- Define Functions -----
    }
}