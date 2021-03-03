using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.InsertProductWindow
{
    /// <summary>
    /// InsertProductWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InsertProductWindow : Window
    {
        public InsertProductWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseInsertProductWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}