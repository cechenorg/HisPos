using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelProductDetailWindow : ProductDetailViewModel, IChromeTabViewModel
    {
        #region ----- Define Variables -----
        private bool _canMoveTabs;
        private bool _showAddButton;

        public double WindowWidth => SystemParameters.WorkArea.Width * 0.8;
        public double WindowHeight => SystemParameters.WorkArea.Height * 0.8;
        public double StartTop => (SystemParameters.WorkArea.Height - WindowHeight) / 2;
        public double StartLeft => (SystemParameters.WorkArea.Width - WindowWidth) / 2;

        public bool CanMoveTabs
        {
            get { return _canMoveTabs; }
            set
            {
                if (_canMoveTabs != value)
                {
                    Set(() => CanMoveTabs, ref _canMoveTabs, value);
                }
            }
        }
        public bool ShowAddButton
        {
            get { return _showAddButton; }
            set
            {
                if (_showAddButton != value)
                {
                    Set(() => ShowAddButton, ref _showAddButton, value);
                }
            }
        }
        #endregion

        public ViewModelProductDetailWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
          
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            CanMoveTabs = true;
            ShowAddButton = false;

            RegisterMessenger();
        }

        #region ----- Define Functions -----
        private void RegisterMessenger()
        {
            Messenger.Default.Register<NotificationMessage<ProductManageStruct>>(this, GetSelectedProductDetail);
        }
        private void GetSelectedProductDetail(NotificationMessage<ProductManageStruct> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductManagementView))
                AddTabCommandAction(notificationMessage.Content);
        }
        #endregion
    }
}
