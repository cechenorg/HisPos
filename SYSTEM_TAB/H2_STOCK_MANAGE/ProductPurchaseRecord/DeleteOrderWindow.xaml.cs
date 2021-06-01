using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    /// <summary>
    /// DeleteOrderWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DeleteOrderWindow : Window
    {
        #region ----- Define Variables -----

        public RelayCommand DeleteOrderCommand { get; set; }

        #endregion ----- Define Variables -----

        #region ----- Define Variables -----

        private string OrderID { get; }
        private string ReceiveID { get; }
        public string CheckStringHint { get { return $"輸入刪除單號 {ReceiveID}"; } }
        public string CheckString { get; set; } = "";

        #endregion ----- Define Variables -----

        public DeleteOrderWindow(string orderID, string receiveID)
        {
            InitializeComponent();
            DataContext = this;

            OrderID = orderID;
            ReceiveID = receiveID;

            DeleteOrderCommand = new RelayCommand(DeleteOrderAction, CanDelete);
        }

        #region ----- Define Actions -----

        private void DeleteOrderAction()
        {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否確認刪除?", "再次確認");

            if ((bool)confirmWindow.DialogResult)
            {
                MainWindow.ServerConnection.OpenConnection();
                DataTable dataTable = StoreOrderDB.DeleteDoneOrder(OrderID);
                MainWindow.ServerConnection.CloseConnection();

             


                if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                    MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
                else if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("LOW"))
                {
                    MessageWindow.ShowMessage("庫存不足 刪除失敗！", MessageType.ERROR);
                }

                else
                    MessageWindow.ShowMessage("網路異常 刪除失敗!", MessageType.ERROR);
            }

            Close();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private bool CanDelete()
        {
            if (!CheckString.Equals(ReceiveID))
                return false;

            return true;
        }

        #endregion ----- Define Functions -----
    }
}