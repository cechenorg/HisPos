using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using System.Data;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// ScrapWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ScrapWindow : Window
    {
        #region ----- Define Variables -----

        private string productID;
        private string wareHouseID;
        private double totalInventory;

        public string ScrapAmount { get; set; }

        #endregion ----- Define Variables -----

        public ScrapWindow(string proID, string wareID, double totalInv)
        {
            InitializeComponent();

            DataContext = this;

            productID = proID;
            wareHouseID = wareID;
            totalInventory = totalInv;
        }

        #region ----- Define Functions -----

        private bool IsScrapValid()
        {
            double tempScrap = 0;
            bool isDouble = double.TryParse(ScrapAmount, out tempScrap);

            if (!isDouble)
            {
                MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                return false;
            }

            if (tempScrap < 0)
            {
                MessageWindow.ShowMessage("輸入數值不可小於0!", MessageType.ERROR);
                return false;
            }

            if (tempScrap > totalInventory)
            {
                MessageWindow.ShowMessage("報廢數量不可大於庫存量!", MessageType.ERROR);
                return false;
            }

            return true;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsScrapValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認報廢數量為 {ScrapAmount} ?\n(報廢後庫存量為 {(totalInventory - double.Parse(ScrapAmount)).ToString("0.##")} )", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductDetailDB.ScrapProductByID(productID, ScrapAmount, wareHouseID);
            MainWindow.ServerConnection.CloseConnection();

            if (!(dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS")))
                MessageWindow.ShowMessage("報廢失敗 請稍後再試", MessageType.ERROR);
            else
                DialogResult = true;

            Close();
        }

        #endregion ----- Define Functions -----
    }
}