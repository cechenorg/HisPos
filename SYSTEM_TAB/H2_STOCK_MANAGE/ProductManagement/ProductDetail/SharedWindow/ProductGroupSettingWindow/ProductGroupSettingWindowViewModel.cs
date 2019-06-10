using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.NewClass.WareHouse;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow.ProductGroupSettingUsercontrol.SplitProductWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VM = His_Pos.ChromeTabViewModel.ViewModelMainWindow;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow {
    public class ProductGroupSettingWindowViewModel : ObservableObject {
        #region Var
        public string WarID { get; set; }
        private bool isSplit = false;
        public bool IsSplit {
            get { return isSplit; }
            set { Set(() => IsSplit, ref isSplit, value); }
        }
        private ProductGroupSetting productGroupSettingSelectedItem;
        public ProductGroupSetting ProductGroupSettingSelectedItem
        {
            get { return productGroupSettingSelectedItem; }
            set { Set(() => ProductGroupSettingSelectedItem, ref productGroupSettingSelectedItem, value); }
        }
        private ProductGroupSettings productGroupSettingCollection = new ProductGroupSettings();
        public ProductGroupSettings ProductGroupSettingCollection
        {
            get { return productGroupSettingCollection; }
            set { Set(() => ProductGroupSettingCollection, ref productGroupSettingCollection, value); }
        }
        public RelayCommand SetIsSpiltTrueCommand { get; set; }
        public RelayCommand SetIsSpiltFalseCommand { get; set; }
        public RelayCommand<string> AddProductByInputCommand { get; set; }
        public RelayCommand MergeProductGroupCommand { get; set; }
        public RelayCommand SplitProductGroupCommand { get; set; }
        public RelayCommand RemoveMergeProductCommand { get; set; }
        #endregion


        public ProductGroupSettingWindowViewModel(string proID,string warID) {
            
            SetIsSpiltTrueCommand = new RelayCommand(SetIsSpiltTrueAction);
            SetIsSpiltFalseCommand = new RelayCommand(SetIsSpiltFalseAction);
            AddProductByInputCommand = new RelayCommand<string>(AddProductByInputAction);
            MergeProductGroupCommand = new RelayCommand(MergeProductGroupAction);
            SplitProductGroupCommand = new RelayCommand(SplitProductGroupAction);
            RemoveMergeProductCommand = new RelayCommand(RemoveMergeProductAction);
            ProductGroupSettingCollection.GetDataByID(proID, warID);
            WarID = warID; 
        }
        #region Function
        private void CloseWindow() {
            Messenger.Default.Send(new NotificationMessage(this, "CloseProductGroupSettingWindow"));
        }
        private void RemoveMergeProductAction() {
            if (ProductGroupSettingSelectedItem is null) return;
            ProductGroupSettingCollection.Remove(ProductGroupSettingSelectedItem);
        }
        private void SplitProductGroupAction() {
            if(ProductGroupSettingSelectedItem is null)
                MessageWindow.ShowMessage("請選擇欲拆出之商品", MessageType.ERROR);
            if (ProductGroupSettingCollection.Count < 2)
            {
                MessageWindow.ShowMessage("商品不可小於兩種", MessageType.ERROR);
                return;
            }

            ConfirmWindow confirmWindow = new ConfirmWindow("是否拆出此商品?", "拆庫確認");
            if (((bool)confirmWindow.DialogResult) == true)
            {
                SplitProductWindow splitProductWindow = new SplitProductWindow(ProductGroupSettingSelectedItem.ID);
                CloseWindow();
            }
        }
        private void MergeProductGroupAction() {
            if (ProductGroupSettingCollection.Count < 2) {
                MessageWindow.ShowMessage("合併商品不可小於兩種", MessageType.ERROR);
                return;
            }
             
            ConfirmWindow confirmWindow = new ConfirmWindow("是否合併庫存?","併庫確認");
            if (((bool)confirmWindow.DialogResult) == true)
            {
                ProductGroupSettingCollection.MergeProduct(WarID);
                MessageWindow.ShowMessage("合併成功",MessageType.SUCCESS);
                CloseWindow();
            }
            
        }
        private void SetIsSpiltTrueAction()  {
            IsSplit = true;
        }
        private void SetIsSpiltFalseAction()
        {
            IsSplit = false;
        }
        private void GetSelectedProduct(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductGroupSettingWindowViewModel))
            {
                if (ProductGroupSettingCollection.Count(p => p.ID == notificationMessage.Content.ID) > 0) {
                    MessageWindow.ShowMessage("已輸入過此商品",MessageType.ERROR);
                    return;
                }
                 
                ProductGroupSettingSelectedItem = new ProductGroupSetting();
                ProductGroupSettingSelectedItem.IsEditable = true;
                ProductGroupSettingSelectedItem.ID = notificationMessage.Content.ID;
                ProductGroupSettingSelectedItem.ChineseName = notificationMessage.Content.ChineseName;
                ProductGroupSettingSelectedItem.EnglishName = notificationMessage.Content.EnglishName;
                ProductGroupSettingCollection.Add(ProductGroupSettingSelectedItem);
            }
        }
        private void AddProductByInputAction(string searchString)
        {
          
            if (searchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            AddProductEnum addProductEnum = AddProductEnum.ProductGroupSetting;

            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(searchString, addProductEnum);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum);
                productPurchaseReturnAddProductWindow.ShowDialog();
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                ProductPurchaseReturnAddProductWindow productPurchaseReturnAddProductWindow = new ProductPurchaseReturnAddProductWindow(searchString, addProductEnum);
                Messenger.Default.Unregister(this);
            }
            else
            {
                MessageWindow.ShowMessage("查無此藥品", MessageType.WARNING);
            }
        }
        #endregion
    }
}
