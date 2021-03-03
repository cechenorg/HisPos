using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelProductDetailWindow : ProductDetailViewModel, IChromeTabViewModel
    {
        #region ----- Define Variables -----

        private bool _canMoveTabs;
        private bool _showAddButton;

        public double WindowWidth => SystemParameters.WorkArea.Width * 0.9;
        public double WindowHeight => SystemParameters.WorkArea.Height * 0.9;
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

        #endregion ----- Define Variables -----

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
            Messenger.Default.Register<NotificationMessage<string[]>>(this, GetSelectedProductDetail);
            Messenger.Default.Register<NotificationMessage>(this, ClearTabs);
        }

        private void GetSelectedProductDetail(NotificationMessage<string[]> notificationMessage)
        {
            if (notificationMessage.Notification == "ShowProductDetail")
                AddTabCommandAction(notificationMessage.Content);
        }

        private void ClearTabs(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == "CloseProductTabs" && notificationMessage.Sender is ProductDetailWindow)
                ItemCollection.Clear();
        }

        #endregion ----- Define Functions -----
    }
}