using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.StockTaking.StockTaking;
using His_Pos.NewClass.WareHouse;
using System;
using System.ComponentModel;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// StockTakingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingWindow : Window, INotifyPropertyChanged
    {
        #region ----- Define Variables -----

        private string productID;
        private WareHouse wareHouse;
        private MedicineStockDetail stockDetail;
        private double newInventory;
        private double shelfInventory;
        private double medInventory;
        private double currentInventory;
        private int autoHeight;
        private int autoGridHeight;
        private bool isbtnEnable;
        private ProductTypeEnum productType;
        public string NewPrice { get; set; }

        public double NewInventory
        {
            get { return newInventory; }
            set
            {
                newInventory = value;
                shelfInventory = NewInventory - stockDetail.MedBagInventory;
                IsbtnEnable = NewInventory > currentInventory ? false : true;
                OnPropertyChanged(nameof(IsOverage));
                OnPropertyChanged(nameof(ShelfInventory));
                OnPropertyChanged(nameof(IsbtnEnable));
            }
        }
        public double ShelfInventory
        {
            get { return shelfInventory; }
            set
            {
                shelfInventory = value;
                newInventory = shelfInventory + stockDetail.MedBagInventory;
                OnPropertyChanged(nameof(IsOverage));
                OnPropertyChanged(nameof(NewInventory));
            }
        }
        public double MedInventory
        {
            get { return medInventory; }
            set { medInventory = value;}
        }
        public int AutoHeight
        {
            get { return autoHeight; }
            set { autoHeight = value; }
        }
        public int AutoGridHeight
        {
            get { return autoGridHeight; }
            set { autoGridHeight = value; }
        }
        public bool IsbtnEnable
        {
            get { return isbtnEnable; }
            set { isbtnEnable = value; }
        }
        public bool IsOverage
        {
            get
            {
                if (NewInventory > stockDetail.ShelfInventory && productType != ProductTypeEnum.Deposit) return true;
                else return false;
            }
        }

        public ProductTypeEnum ProductType
        {
            get { return productType; }
            set { productType = value; }
        }

        #endregion ----- Define Variables -----

        public StockTakingWindow(string proID, WareHouse ware, MedicineStockDetail stock, bool isOTCType, ProductTypeEnum productType)
        {
            InitializeComponent();
            DataContext = this;
            productID = proID;
            wareHouse = ware;
            stockDetail = stock;
            NewPrice = productType == ProductTypeEnum.Deposit ? "0.00" : stock.LastPrice.ToString("0.##");
            newInventory = stock.ShelfInventory + stock.MedBagInventory;
            currentInventory = stock.ShelfInventory + stock.MedBagInventory;
            shelfInventory = stock.ShelfInventory;
            MedInventory = stock.MedBagInventory;
            AutoHeight = isOTCType ? 60 : 200;
            AutoGridHeight = isOTCType ? 0 : 50;
            ProductType = productType;
            IsbtnEnable = true;
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
                if (NewInventory < 0)
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

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將總庫存調整為 {NewInventory.ToString("0.##")} ?", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            StockTaking stockTaking = new StockTaking();
            if (ProductType == ProductTypeEnum.Deposit)
                NewPrice = "0";
            stockTaking.SingleStockTaking(productID, stockDetail.TotalInventory, NewInventory, double.Parse(NewPrice), wareHouse);
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
    }
}