using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.PurchaseReturn;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.ChooseBatchWindow
{
    public class ChooseBatchWindowViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand<string> SelectBatchCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        public string ChoosedBatchNumber { get; set; }
        public Collection<string> BatchNumberCollection { get; set; }
        public string Title { get; }
        public bool IsSelected { get; set; } = false;
        #endregion

        public ChooseBatchWindowViewModel(string productID, string wareID)
        {
            Title = productID + " 批號選擇";

            InitBatchCollection(productID, wareID);
            SelectBatchCommand = new RelayCommand<string>(SelectBatchAction);
        }

        #region ----- Define Actions -----
        private void SelectBatchAction(string batch)
        {
            IsSelected = true;

            ChoosedBatchNumber = batch;

            Messenger.Default.Send(new NotificationMessage("CloseChooseBatchWindow"));
        }
        #endregion

        #region ----- Define Functions -----
        private void InitBatchCollection(string productID, string wareID)
        {
            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = PurchaseReturnProductDB.GetReturnProductBatchNumbers(productID, wareID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable.Rows.Count == 0)
            {
                MessageWindow.ShowMessage("無批號資料!", MessageType.ERROR);
                Messenger.Default.Send(new NotificationMessage("CloseChooseBatchWindow"));
            }

            BatchNumberCollection = new Collection<string>();

            foreach (DataRow row in dataTable.Rows)
                BatchNumberCollection.Add(row.Field<string>("BatchNumber"));
        }
        #endregion
    }
}
