using GalaSoft.MvvmLight.Messaging;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.ManufactoryManage.AddManufactoryWindow
{
    /// <summary>
    /// AddManufactoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AddManufactoryWindow : Window
    {
        public AddManufactoryWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAddManufactoryWindow") && notificationMessage.Sender is AddManufactoryWindowViewModel)
                    Close();
            });

            Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.V)
            {
                e.Handled = true;
            }
        }
    }
}