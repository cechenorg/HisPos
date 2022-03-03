using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.StoreOrder;
using System.Windows;
using DomainModel.Enum;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// ProductPurchaseReturnAddProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseReturnAddProductWindow : Window
    {
        public ProductPurchaseReturnAddProductWindow(string searchString, AddProductEnum addProductEnum, OrderStatusEnum OrderStatus, string wareID = "0")
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });

            if (searchString.Equals(""))
                DataContext = new AddProductViewModel(addProductEnum, wareID, OrderStatus);
            else
                DataContext = new AddProductViewModel(searchString, addProductEnum, wareID, OrderStatus);

            SearchStringTextBox.Focus();

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        public ProductPurchaseReturnAddProductWindow(string searchString, AddProductEnum addProductEnum, string wareID = "0")
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });

            if (searchString.Equals(""))
                DataContext = new AddProductViewModel(addProductEnum, wareID);
            else
                DataContext = new AddProductViewModel(searchString, addProductEnum, wareID);

            SearchStringTextBox.Focus();

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        public ProductPurchaseReturnAddProductWindow(string searchString, AddProductEnum addProductEnum, OrderStatusEnum OrderStatus, string wareID, string orderTypeIsOTC ) 
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });

            if (searchString.Equals(""))
                DataContext = new AddProductViewModel(addProductEnum, wareID, OrderStatus);
            else
                DataContext = new AddProductViewModel(searchString, addProductEnum, wareID, OrderStatus, orderTypeIsOTC);

            SearchStringTextBox.Focus();

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}