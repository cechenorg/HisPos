using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement;

namespace His_Pos.ChromeTabViewModel
{
    public class ViewModelProductDetailWindow : ProductDetailViewModel, IChromeTabViewModel
    {
        private bool _canMoveTabs;
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
        //this property is to show you can bind the visibility of the add button
        private bool _showAddButton;
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

        public ViewModelProductDetailWindow()
        {
            SelectedTab = ItemCollection.FirstOrDefault();
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemCollection);
          
            //This sort description is what keeps the source collection sorted, based on tab number. 
            //You can also use the sort description to manually sort the tabs, based on your own criterias.
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));

            CanMoveTabs = true;
            ShowAddButton = false;

            RegisterMessenger();
        }

        #region ----- Define Command -----
        #endregion

        #region ----- Define Variables -----
        #endregion

        #region ----- Define Actions -----
        #endregion

        #region ----- Define Functions -----
        private void RegisterMessenger()
        {
            Messenger.Default.Register<NotificationMessage<string>>(this, GetSelectedProductDetail);
        }
        private void GetSelectedProductDetail(NotificationMessage<string> notificationMessage)
        {
            if (notificationMessage.Notification == nameof(ProductManagementView))
            {


                //MainWindow.ServerConnection.OpenConnection();

                //MainWindow.ServerConnection.CloseConnection();
            }
        }
        #endregion
    }
}
