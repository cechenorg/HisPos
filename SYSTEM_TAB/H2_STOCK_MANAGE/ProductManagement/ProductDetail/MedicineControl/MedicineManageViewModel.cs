using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Medicine.Base;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.NewClass.StockTaking.StockTaking;
using His_Pos.NewClass.StockTaking.StockTakingProduct;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.GroupControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.PriceControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.RecordControl;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    public class MedicineManageViewModel : ViewModelBase
    {
        #region ----- Define ViewModels -----
        public GroupInventoryControlViewModel GroupViewModel { get; set; }
        public SingdePriceControlViewModel PriceViewModel { get; set; }
        public ProductRecordDetailControlViewModel RecordViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand StockTakingCommand { get; set; }
        public RelayCommand RecycleCommand { get; set; }
        public RelayCommand ScrapCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string newInventory = "";
        private string recycleAmount = "";
        private string scrapAmount = "";
        private WareHouse selectedWareHouse;

        public WareHouses WareHouseCollection { get; set; }
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value);
                //SearchProductRecordAction();
            }
        }
        public string NewInventory
        {
            get { return newInventory; }
            set
            {
                Set(() => NewInventory, ref newInventory, value);
            }
        }
        public string RecycleAmount
        {
            get { return recycleAmount; }
            set
            {
                Set(() => RecycleAmount, ref recycleAmount, value);
            }
        }
        public string ScrapAmount
        {
            get { return scrapAmount; }
            set
            {
                Set(() => ScrapAmount, ref scrapAmount, value);
            }
        }
        #endregion

        public MedicineManageViewModel()
        {
            RegisterCommand();
        }

        #region ----- Define Actions -----
        private void ScrapAction()
        {
            if (!IsScrapValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認報廢數量為 {ScrapAmount} ?\n(報廢後庫存量為 {(StockDetail.TotalInventory - double.Parse(ScrapAmount)).ToString("0.##")} )", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductDetailDB.ScrapProductByID(Medicine.ID, ScrapAmount, SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            if (!(dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS")))
                MessageWindow.ShowMessage("報廢失敗 請稍後再試", MessageType.ERROR);

            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);

            ScrapAmount = "";
        }
        private void RecycleAction()
        {
            if (!IsRecycleValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認回收數量為 {RecycleAmount} ?\n(回收後庫存量為 {(StockDetail.TotalInventory + double.Parse(RecycleAmount)).ToString("0.##")} )", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductDetailDB.RecycleProductByID(Medicine.ID, RecycleAmount, SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            if (!(dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS")))
                MessageWindow.ShowMessage("回收失敗 請稍後再試", MessageType.ERROR);

            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);

            RecycleAmount = "";
        }
        private void StockTakingAction()
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

            stockTakingProduct.ID = Medicine.ID;
            stockTakingProduct.Inventory = StockDetail.TotalInventory;
            stockTakingProduct.NewInventory = double.Parse(NewInventory);
            stockTaking.StockTakingProductCollection.Add(stockTakingProduct);
            stockTaking.InsertStockTaking("單品盤點");
            //ProductDetailDB.StockTakingProductManageMedicineByID(Medicine.ID, NewInventory,SelectedWareHouse.ID);
            MainWindow.ServerConnection.CloseConnection();

            InitMedicineData(Medicine.ID, SelectedWareHouse.ID);

            NewInventory = "";
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("UpdateUsableAmountMessage"));
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommand()
        {
            StockTakingCommand = new RelayCommand(StockTakingAction);
            ScrapCommand = new RelayCommand(ScrapAction);
            RecycleCommand = new RelayCommand(RecycleAction);
        }
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

            if (StockDetail.MedBagInventory - double.Parse(NewInventory) > 0 && StockDetail.OnTheWayAmount < StockDetail.MedBagInventory - double.Parse(NewInventory))
            {
                MessageWindow.ShowMessage("若欲盤點使庫存量低於藥袋量，請先建立採購單補足藥袋量!", MessageType.ERROR);
                return false;
            }

            return true;
        }
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

            if (tempScrap > StockDetail.TotalInventory)
            {
                MessageWindow.ShowMessage("報廢數量不可大於庫存量!", MessageType.ERROR);
                return false;
            }

            return true;
        }
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
        #endregion
    }
}
