using GalaSoft.MvvmLight.CommandWpf;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System;
using System.Data;
using System.Globalization;
using System.Threading;
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
        public string CheckStringHint { get { return $"輸入刪除單號 {OrderID}"; } }
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
            btnDelete.IsEnabled = false;
            int type = OrderID.Length > 0 && OrderID.Substring(0, 1) == "P" ? 0 : 1;
            ScrapOrderWindow ScrapOrderWindow = new ScrapOrderWindow(type);
            ScrapOrderWindowViewModel ScrapOrder = (ScrapOrderWindowViewModel)ScrapOrderWindow.DataContext;
            if ((bool)ScrapOrderWindow.DialogResult)
            {
                ConfirmWindow confirmWindow = new ConfirmWindow("是否確認刪除?", "再次確認");
                if ((bool)confirmWindow.DialogResult)
                {
                    string voidReason = ScrapOrder.Content + ScrapOrder.Other;
                    MainWindow.ServerConnection.OpenConnection();
                    DataTable dataTable = StoreOrderDB.DeleteDoneOrder(OrderID, voidReason);
                    MainWindow.ServerConnection.CloseConnection();

                    if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
                    {
                        MessageWindow.ShowMessage("刪除成功", MessageType.SUCCESS);
                    }
                    else if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("LOW"))
                    {
                        MessageWindow.ShowMessage("庫存不足 刪除失敗！", MessageType.ERROR);
                    }
                    else if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("STRIKED"))
                    {
                        MessageWindow.ShowMessage("刪除失敗!已沖帳過!", MessageType.ERROR);
                    }

                    else
                        MessageWindow.ShowMessage("網路異常 刪除失敗!", MessageType.ERROR);
                }
            }
            
            Close();
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private bool CanDelete()
        {
            if (!CheckString.Equals(OrderID))
                return false;

            return true;
        }

        #endregion ----- Define Functions -----
    }
}