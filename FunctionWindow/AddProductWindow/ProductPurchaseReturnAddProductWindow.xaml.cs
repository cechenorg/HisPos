using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.FunctionWindow.AddProductWindow
{
    /// <summary>
    /// ProductPurchaseReturnAddProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductPurchaseReturnAddProductWindow : Window
    {
        public ProductPurchaseReturnAddProductWindow(string searchString, AddProductEnum addProductEnum)
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddProductView"))
                    Close();
            });

            if (searchString.Equals(""))
                DataContext = new AddProductViewModel(addProductEnum);
            else
                DataContext = new AddProductViewModel(searchString, addProductEnum);

            SearchStringTextBox.Focus();

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
