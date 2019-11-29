using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using His_Pos.NewClass.StoreOrder.SingdeTotalOrder;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn
{
    public class SingdeTotalViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand<string> ToDoneCommand { get; set; }
        public RelayCommand AllToDoneCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private SingdeTotalOrder currenTotalOrder;

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
        #endregion

        public SingdeTotalViewModel()
        {
            InitCommands();

        }

        #region ----- Define Actions -----
        private void ToDoneAction(string id)
        {

        }
        private void AllToDoneAction()
        {

        }
        #endregion

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
        }
        #endregion
    }
}
