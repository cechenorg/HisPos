using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.AddNewOrderWindow
{
    /// <summary>
    /// AddNewOrderWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddNewOrderWindow : Window
    {
        public AddNewOrderWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseAddNewOrderWindow")
                    Close();
            });
        }
    }
}