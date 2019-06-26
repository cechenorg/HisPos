using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.NewClass.WareHouse;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow {
    public class ProductGroupSettingWindowViewModel : ViewModelBase
    {
        #region ----- Define Commands -----
        public RelayCommand MergeProductGroupCommand { get; set; }
        public RelayCommand SplitProductGroupCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private bool isMergeProduct = true;
        private string wareHouseID;
        private ProductGroupSettings productGroupSettingCollection;
        private ProductGroupSetting currentProductGroupSetting;

        public bool IsMergeProduct
        {
            get { return isMergeProduct; }
            set { Set(() => IsMergeProduct, ref isMergeProduct, value); }
        }
        public ProductGroupSettings ProductGroupSettingCollection
        {
            get { return productGroupSettingCollection; }
            set { Set(() => ProductGroupSettingCollection, ref productGroupSettingCollection, value); }
        }
        public ProductGroupSetting CurrentProductGroupSetting
        {
            get { return currentProductGroupSetting; }
            set { Set(() => CurrentProductGroupSetting, ref currentProductGroupSetting, value); }
        }
        public double TotalInventory { get; set; }
        public double SplitAmount { get; set; }
        #endregion

        public ProductGroupSettingWindowViewModel(ProductGroupSettings productGroupSettingCollection, string wareID, double inventory)
        {
            ProductGroupSettingCollection = productGroupSettingCollection;
            wareHouseID = wareID;
            TotalInventory = inventory;

            RegisterCommands();
        }

        #region ----- Define Actions -----
        private void MergeProductGroupAction()
        {

        }
        private void SplitProductGroupAction()
        {
            if(IsSplitValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將 {CurrentProductGroupSetting.ID} 從群組中拆出\n拆出數量為 {SplitAmount}", "");

            if(!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductGroupSettingDB.SplitProduct(CurrentProductGroupSetting.ID, SplitAmount, wareHouseID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable?.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("拆分成功", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseProductGroupSettingWindow"));
            }
            else
                MessageWindow.ShowMessage("拆分失敗 請稍後再試", MessageType.ERROR);
        }
        #endregion

        #region ----- Define Functions -----
        private void RegisterCommands()
        {
            MergeProductGroupCommand = new RelayCommand(MergeProductGroupAction);
            SplitProductGroupCommand = new RelayCommand(SplitProductGroupAction);
        }
        private bool IsSplitValid()
        {
            if (SplitAmount > TotalInventory)
            {
                MessageWindow.ShowMessage("拆分量不得大於庫存量", MessageType.ERROR);
                return false;
            }

            if (SplitAmount < 0)
            {
                MessageWindow.ShowMessage("拆分量不得小於0", MessageType.ERROR);
                return false;
            }

            if (CurrentProductGroupSetting is null)
            {
                MessageWindow.ShowMessage("必須指定拆分品項", MessageType.ERROR);
                return false;
            }

            if(ProductGroupSettingCollection.Count == 1)
            {
                MessageWindow.ShowMessage("已剩最後一個品項 無法拆分", MessageType.ERROR);
                return false;
            }

            return true;
        }
        #endregion
    }
}
