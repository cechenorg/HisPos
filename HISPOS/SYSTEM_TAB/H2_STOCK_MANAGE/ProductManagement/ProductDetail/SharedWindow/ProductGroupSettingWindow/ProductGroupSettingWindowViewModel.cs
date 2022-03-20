using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.FunctionWindow.AddProductWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.ProductGroupSetting;
using System.Data;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow
{
    public class ProductGroupSettingWindowViewModel : ViewModelBase
    {
        #region ----- Define Commands -----

        public RelayCommand MergeProductGroupCommand { get; set; }
        public RelayCommand SplitProductGroupCommand { get; set; }
        public RelayCommand SearchMergeProductCommand { get; set; }

        #endregion ----- Define Commands -----

        #region ----- Define Variables -----

        private bool isMergeProduct = true;
        private string wareHouseID;
        private ProductGroupSettings productGroupSettingCollection;
        private ProductGroupSetting currentProductGroupSetting;
        private ProductStruct mergeProductStruct;
        private string searchString = "";

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

        public string SearchString
        {
            get { return searchString; }
            set { Set(() => SearchString, ref searchString, value); }
        }

        #endregion ----- Define Variables -----

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
            SearchMergeProductAction();

            if (!IsMergeValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將 {mergeProductStruct.ID} 併入群組\n合併後庫存量為 {mergeProductStruct.Inventory + TotalInventory}", "");

            if (!(bool)confirmWindow.DialogResult) return;

            MainWindow.ServerConnection.OpenConnection();
            DataTable dataTable = ProductGroupSettingDB.MergeProduct(ProductGroupSettingCollection[0].ID, mergeProductStruct.ID, wareHouseID);
            MainWindow.ServerConnection.CloseConnection();

            if (dataTable?.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                MessageWindow.ShowMessage("合併成功", MessageType.SUCCESS);
                Messenger.Default.Send(new NotificationMessage("CloseProductGroupSettingWindow"));
            }
            else
                MessageWindow.ShowMessage("合併失敗 請稍後再試", MessageType.ERROR);
        }

        private void SplitProductGroupAction()
        {
            if (!IsSplitValid()) return;

            ConfirmWindow confirmWindow = new ConfirmWindow($"是否確認將 {CurrentProductGroupSetting.ID} 從群組中拆出\n拆出數量為 {SplitAmount}", "");

            if (!(bool)confirmWindow.DialogResult) return;

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

        private void SearchMergeProductAction()
        {
            if (SearchString.Length < 5)
            {
                MessageWindow.ShowMessage("搜尋字長度不得小於5", MessageType.WARNING);
                return;
            }

            AddProductEnum addProductEnum = AddProductEnum.ProductGroupSetting;

            MainWindow.ServerConnection.OpenConnection();
            var productCount = ProductStructs.GetProductStructCountBySearchString(SearchString, addProductEnum, wareHouseID);
            MainWindow.ServerConnection.CloseConnection();
            if (productCount > 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                AddMedicineWindow addMedicineWindow = new AddMedicineWindow(SearchString, addProductEnum, wareHouseID);
                addMedicineWindow.ShowDialog();
                Messenger.Default.Unregister(this);
            }
            else if (productCount == 1)
            {
                Messenger.Default.Register<NotificationMessage<ProductStruct>>(this, GetSelectedProduct);
                AddMedicineWindow addMedicineWindow = new AddMedicineWindow(SearchString, addProductEnum, wareHouseID);
                Messenger.Default.Unregister(this);
            }
            else
            {
                MessageWindow.ShowMessage("查無此商品", MessageType.WARNING);
            }
        }

        #endregion ----- Define Actions -----

        #region ----- Define Functions -----

        private void RegisterCommands()
        {
            MergeProductGroupCommand = new RelayCommand(MergeProductGroupAction);
            SplitProductGroupCommand = new RelayCommand(SplitProductGroupAction);
            SearchMergeProductCommand = new RelayCommand(SearchMergeProductAction);
        }

        private bool IsSplitValid()
        {
            if (ProductGroupSettingCollection.Count == 1)
            {
                MessageWindow.ShowMessage("已剩最後一個品項 無法拆分", MessageType.ERROR);
                return false;
            }

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

            return true;
        }

        private bool IsMergeValid()
        {
            if (mergeProductStruct.Inventory < 0)
            {
                MessageWindow.ShowMessage("合併商品庫存不得小於0", MessageType.ERROR);
                return false;
            }

            if (mergeProductStruct.ID is null)
            {
                MessageWindow.ShowMessage("必須選擇合併品項", MessageType.ERROR);
                return false;
            }

            foreach (var product in ProductGroupSettingCollection)
            {
                if (mergeProductStruct.ID.Equals(product.ID))
                {
                    MessageWindow.ShowMessage("不可新增已存在品項", MessageType.ERROR);
                    return false;
                }
            }

            return true;
        }

        private void GetSelectedProduct(NotificationMessage<ProductStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductGroupSettingWindowViewModel))
            {
                mergeProductStruct = notificationMessage.Content;
                SearchString = mergeProductStruct.ID;
            }
        }

        #endregion ----- Define Functions -----
    }
}