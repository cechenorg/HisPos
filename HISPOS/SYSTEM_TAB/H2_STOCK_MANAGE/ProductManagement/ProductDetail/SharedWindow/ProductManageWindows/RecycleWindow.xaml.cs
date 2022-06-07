using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// RecycleWindow.xaml 的互動邏輯
    /// </summary>
    public partial class RecycleWindow : Window
    {
        #region ----- Define Variables -----

        private string productID;
        private string wareHouseID;
        private double totalInventory;

        public string RecycleAmount { get; set; }

        #endregion ----- Define Variables -----

        public RecycleWindow(string proID, string wareID, double totalInv)
        {
            InitializeComponent();

            DataContext = this;

            productID = proID;
            wareHouseID = wareID;
            totalInventory = totalInv;
        }

        #region ----- Define Functions -----

        private bool IsRecycleValid()
        {
            double tempRecycle = 0;
            bool isDouble = double.TryParse(RecycleAmount, out tempRecycle);

            if (!isDouble)
            {
                MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                return false;
            }

            if (tempRecycle < 0)
            {
                MessageWindow.ShowMessage("輸入數值不可小於0!", MessageType.ERROR);
                return false;
            }

            return true;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsRecycleValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認回收數量為 {RecycleAmount} ?\n(回收後庫存量為 {(totalInventory + double.Parse(RecycleAmount)).ToString("0.##")} )", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductDetailDB.RecycleProductByID(productID, RecycleAmount, wareHouseID);
            MainWindow.ServerConnection.CloseConnection();

            if (!(dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS")))
                MessageWindow.ShowMessage("回收失敗 請稍後再試", MessageType.ERROR);
            else
                DialogResult = true;

            Close();
        }

        #endregion ----- Define Functions -----
    }
}