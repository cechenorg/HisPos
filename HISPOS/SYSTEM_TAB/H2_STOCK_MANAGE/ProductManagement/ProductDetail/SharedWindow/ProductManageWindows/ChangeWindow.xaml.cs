using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.StockTaking.StockTaking;
using His_Pos.NewClass.WareHouse;
using System.ComponentModel;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// StockTakingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChangeWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----

        private string productID;
        private WareHouse wareHouse;
        private MedicineStockDetail stockDetail;
        private string newInventory;
        private string number;
        private bool isOverage;
        public int Min { get; set; } = 1;
        public string CHANGE { get; set; } = "轉讓";
        public string NewPrice { get; set; }

        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                newInventory = value;
                OnPropertyChanged(nameof(NewInventory));
            }
        }

        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                OnPropertyChanged(nameof(Number));
            }
        }

        public bool IsOverage
        {
            get { return isOverage; }
            set
            {
                isOverage = value;
                OnPropertyChanged(nameof(IsOverage));
            }
        }

        #endregion ----- Define Variables -----

        public ChangeWindow(string proID, WareHouse ware, MedicineStockDetail stock)
        {
            InitializeComponent();

            DataContext = this;

            productID = proID;
            wareHouse = ware;
            stockDetail = stock;
            NewPrice = stock.LastPrice.ToString("0.##");
        }

        #region ----- Define Functions -----

        private bool IsNewInventoryValid()
        {
            if (NewInventory.Equals(""))
            {
                MessageWindow.ShowMessage("盤點架上量不得為空!", MessageType.ERROR);
                return false;
            }
            else
            {
                double newCheckedInventory = 0;
                bool isDouble = double.TryParse(NewInventory, out newCheckedInventory);

                if (!isDouble)
                {
                    MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                    return false;
                }

                if (newCheckedInventory < 0)
                {
                    MessageWindow.ShowMessage("輸入數值不可小於0!", MessageType.ERROR);
                    return false;
                }
            }

            if (NewPrice.Equals("") && IsOverage)
            {
                MessageWindow.ShowMessage("盤點單價不得為空!", MessageType.ERROR);
                return false;
            }
            else if (IsOverage)
            {
                double newCheckedPrice = 0;
                bool isDouble = double.TryParse(NewPrice, out newCheckedPrice);

                if (!isDouble)
                {
                    MessageWindow.ShowMessage("輸入數值錯誤!", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }

        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsNewInventoryValid()) return;

            if (Number == null || Number == "")
            {
                MessageWindow.ShowMessage("必須輸入批號!", MessageType.ERROR);
                return;
            }

            double finalInv = double.Parse(NewInventory);
            double tempShelfInv = stockDetail.TotalInventory - stockDetail.ShelfInventory + double.Parse(NewInventory) - stockDetail.MedBagInventory;
            double shelfInv = (tempShelfInv < 0) ? 0 : tempShelfInv;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否{CHANGE}: {finalInv}", "確認", true);

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            StockTaking stockTaking = new StockTaking();
            stockTaking.SingleStockChange(productID, stockDetail.TotalInventory, stockDetail.TotalInventory + (finalInv * Min), double.Parse(NewPrice), wareHouse, Number);
            MainWindow.ServerConnection.CloseConnection();

            ProductDetailDB.UpdateProductLastPrice(productID, double.Parse(NewPrice), wareHouse.ID);

            DialogResult = true;
            Close();
        }

        #endregion ----- Define Functions -----

        #region ----- Define PropertyChanged -----

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion ----- Define PropertyChanged -----

        private void OUT_Checked(object sender, RoutedEventArgs e)
        {
            IsOverage = false;
            Min = -1;
            CHANGE = "轉讓";
        }

        private void IN_Checked_1(object sender, RoutedEventArgs e)
        {
            IsOverage = true;
            Min = 1;
            CHANGE = "受讓";
        }
    }
}