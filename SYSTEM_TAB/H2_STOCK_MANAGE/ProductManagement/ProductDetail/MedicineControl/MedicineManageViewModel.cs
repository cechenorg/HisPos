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
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.PresControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl.StockControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.GroupControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.PriceControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.RecordControl;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductManageWindows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.MedicineControl
{
    public class MedicineManageViewModel : ViewModelBase
    {
        #region ----- Define ViewModels -----
        public GroupInventoryControlViewModel GroupViewModel { get; set; }
        public SingdePriceControlViewModel PriceViewModel { get; set; }
        public ProductRecordDetailControlViewModel RecordViewModel { get; set; }
        public MedicineStockViewModel StockViewModel { get; set; }
        public PrescriptionViewModel PrescriptionViewModel { get; set; }
        #endregion

        #region ----- Define Commands -----
        public RelayCommand StockTakingCommand { get; set; }
        public RelayCommand RecycleCommand { get; set; }
        public RelayCommand ScrapCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string medicineID;
        private ProductTypeEnum productType;
        private WareHouse selectedWareHouse;

        public WareHouses WareHouseCollection { get; set; }
        public WareHouse SelectedWareHouse
        {
            get { return selectedWareHouse; }
            set
            {
                Set(() => SelectedWareHouse, ref selectedWareHouse, value);
                ReloadData();
            }
        }
        public ProductTypeEnum ProductType
        {
            get { return productType; }
            set { Set(() => ProductType, ref productType, value); }
        }
        #endregion

        public MedicineManageViewModel()
        {
            InitViewModels();
            RegisterCommand();

            WareHouseCollection = WareHouses.GetWareHouses();
            if (WareHouseCollection is null || WareHouseCollection.Count == 0)
            {
                MessageWindow.ShowMessage("網路異常 請稍後再試", MessageType.ERROR);
                return;
            }

            Messenger.Default.Register<NotificationMessage<string>>(this, GetReloadMessage);
        }

        #region ----- Define Actions -----
        private void ScrapAction()
        {
            ScrapWindow scrapWindow = new ScrapWindow();
            scrapWindow.ShowDialog();

            if (scrapWindow.DialogResult != null && (bool)scrapWindow.DialogResult)
                ReloadData();
        }
        private void RecycleAction()
        {
            RecycleWindow recycleWindow = new RecycleWindow();
            recycleWindow.ShowDialog();

            if (recycleWindow.DialogResult != null && (bool)recycleWindow.DialogResult)
                ReloadData();
        }
        private void StockTakingAction()
        {
            StockTakingWindow stockTakingWindow = new StockTakingWindow();
            stockTakingWindow.ShowDialog();

            if(stockTakingWindow.DialogResult != null && (bool)stockTakingWindow.DialogResult)
                ReloadData();
        }
        #endregion

        #region ----- Define Functions -----
        private void InitViewModels()
        {
            GroupViewModel = new GroupInventoryControlViewModel();
            PriceViewModel = new SingdePriceControlViewModel();
            RecordViewModel = new ProductRecordDetailControlViewModel();
            StockViewModel = new MedicineStockViewModel();
            PrescriptionViewModel = new PrescriptionViewModel();
        }
        private void RegisterCommand()
        {
            StockTakingCommand = new RelayCommand(StockTakingAction);
            ScrapCommand = new RelayCommand(ScrapAction);
            RecycleCommand = new RelayCommand(RecycleAction);
        }
        private void ReloadData()
        {
            PriceViewModel.ReloadData(medicineID, selectedWareHouse.ID, productType);
            StockViewModel.ReloadData(medicineID, selectedWareHouse.ID);
            GroupViewModel.ReloadData(medicineID, selectedWareHouse.ID, StockViewModel.StockDetail.TotalInventory);
            RecordViewModel.ReloadData(medicineID, selectedWareHouse.ID);
            PrescriptionViewModel.ReloadData(medicineID, selectedWareHouse.ID);
        }
        private void GetReloadMessage(NotificationMessage<string> message)
        {
            if (message.Sender is GroupInventoryControlViewModel || message.Sender is ProductRecordDetailControlViewModel)
            {
                if(message.Notification.Equals("RELOAD") && message.Content.Equals(medicineID))
                    ReloadData();
            }
        }
        public void ReloadData(string proID, string wareID, ProductTypeEnum type)
        {
            if (!wareID.Equals(String.Empty))
                selectedWareHouse = WareHouseCollection[int.Parse(wareID)];

            medicineID = proID;
            productType = type;

            ReloadData();
        }
        #endregion
    }
}
