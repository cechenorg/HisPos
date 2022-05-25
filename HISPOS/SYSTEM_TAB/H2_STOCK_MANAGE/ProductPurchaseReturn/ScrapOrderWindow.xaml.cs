using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    public partial class ScrapOrderWindow : Window
    {
        public ScrapOrderWindow()
        {
            InitializeComponent();
            DataContext = new ScrapOrderWindowViewModel();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "YesAction":
                        if (radio4.IsChecked == true && other.Text == string.Empty)
                        {
                            MessageWindow.ShowMessage("請填寫其他", MessageType.ERROR);
                            return;
                        }
                        DialogResult = true;
                        Close();
                        break;

                    case "NoAction":
                        DialogResult = false;
                        Close();
                        break;
                }
            });
            ShowDialog();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }

        private void radio_Click(object sender, RoutedEventArgs e)
        {
            if(radio4.IsChecked == true)
            {
                other.Visibility = Visibility.Visible;
            }
            else
            {
                other.Text = "";
                other.Visibility = Visibility.Hidden;
            }
        }
    }
}