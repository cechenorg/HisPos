using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.ProductGroupSetting;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow.ProductGroupSettingUsercontrol.SplitProductWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow {
    public class ProductGroupSettingWindowViewModel : ObservableObject {
        #region Var
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
        #endregion


        public ProductGroupSettingWindowViewModel(string proID) {
            SetIsSpiltTrueCommand = new RelayCommand(SetIsSpiltTrueAction);
            SetIsSpiltFalseCommand = new RelayCommand(SetIsSpiltFalseAction);
            AddProductByInputCommand = new RelayCommand<string>(AddProductByInputAction);
            MergeProductGroupCommand = new RelayCommand(MergeProductGroupAction);
            SplitProductGroupCommand = new RelayCommand(SplitProductGroupAction);
            ProductGroupSettingCollection.GetDataByID(proID);
        }
        #region Function
        private void SplitProductGroupAction() {
            if(ProductGroupSettingSelectedItem is null)
                MessageWindow.ShowMessage("請選擇欲拆出之商品", MessageType.ERROR);
            SplitProductWindow splitProductWindow = new SplitProductWindow(ProductGroupSettingSelectedItem.ProID);
        }
        private void MergeProductGroupAction() {
            ConfirmWindow confirmWindow = new ConfirmWindow("是否合併庫存?","併庫確認");
            if (((bool)confirmWindow.DialogResult) == true)
            {
                ProductGroupSettingCollection.MergeProduct();
                MessageWindow.ShowMessage("合併成功",MessageType.SUCCESS);
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
                ProductGroupSettingSelectedItem = new ProductGroupSetting();
                ProductGroupSettingSelectedItem.IsEditable = true;
                ProductGroupSettingSelectedItem.ProID = notificationMessage.Content.ID;
                ProductGroupSettingSelectedItem.Name = notificationMessage.Content.ChineseName;
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
