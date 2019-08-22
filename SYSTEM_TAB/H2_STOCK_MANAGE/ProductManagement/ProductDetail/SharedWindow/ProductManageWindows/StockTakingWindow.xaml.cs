using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.Product.ProductManagement.ProductStockDetail;
using His_Pos.NewClass.StockTaking.StockTaking;
using His_Pos.NewClass.StockTaking.StockTakingProduct;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows
{
    /// <summary>
    /// StockTakingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StockTakingWindow : Window
    {
        #region ----- Define Variables -----
        private string productID;
        private string wareHouseID;
        private MedicineStockDetail stockDetail;

        public string NewInventory { get; set; }
        #endregion

        public StockTakingWindow(string proID, string wareID, MedicineStockDetail stock)
        {
            InitializeComponent();

            DataContext = this;

            productID = proID;
            wareHouseID = wareID;
            stockDetail = stock;
        }

        #region ----- Define Functions -----
        private bool IsNewInventoryValid()
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

            if (stockDetail.MedBagInventory - double.Parse(NewInventory) > 0 && stockDetail.OnTheWayAmount < stockDetail.MedBagInventory - double.Parse(NewInventory))
            {
                MessageWindow.ShowMessage("若欲盤點使庫存量低於藥袋量，請先建立採購單補足藥袋量!", MessageType.ERROR);
                return false;
            }

            return true;
        }
        private void Confirm_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsNewInventoryValid()) return;

            //if (StockDetail.LastPrice == 0.0)
            //{
            //    StockTakingNoLastPriceWindow stockTakingNoLastPriceWindow = new StockTakingNoLastPriceWindow();
            //    stockTakingNoLastPriceWindow.ShowDialog();

            //    if (stockTakingNoLastPriceWindow.ConfirmClicked)
            //    {
            //        MainWindow.ServerConnection.OpenConnection();
            //        ProductDetailDB.UpdateProductLastPrice(Medicine.ID, stockTakingNoLastPriceWindow.Price, SelectedWareHouse.ID);
            //        MainWindow.ServerConnection.CloseConnection();
            //    }
            //    else
            //        return;
            //}

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將庫存調整為 {NewInventory} ?", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            StockTaking stockTaking = new StockTaking();
            stockTaking.WareHouse = SelectedWareHouse;
            StockTakingProduct stockTakingProduct = new StockTakingProduct();

            stockTakingProduct.ID = medicineID;
            stockTakingProduct.Inventory = StockViewModel.StockDetail.TotalInventory;
            stockTakingProduct.NewInventory = double.Parse(NewInventory);
            stockTaking.StockTakingProductCollection.Add(stockTakingProduct);
            stockTaking.InsertStockTaking("單品盤點");
            ProductDetailDB.StockTakingProductManageMedicineByID(Medicine.ID, NewInventory,SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            if (!(dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS")))
                MessageWindow.ShowMessage("報廢失敗 請稍後再試", MessageType.ERROR);
            else
                DialogResult = true;

            Close();
        }
        #endregion
    }
}
