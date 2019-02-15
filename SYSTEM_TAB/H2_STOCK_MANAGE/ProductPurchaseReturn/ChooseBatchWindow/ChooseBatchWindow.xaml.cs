using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.ChooseBatchWindow
{
    /// <summary>
    /// ChooseBatchWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChooseBatchWindow : Window
    {
        public ChooseBatchWindow(string iD)
        {
            InitializeComponent();

            ChooseBatchWindowViewModel chooseBatchWindowViewModel = new ChooseBatchWindowViewModel(iD);
            DataContext = chooseBatchWindowViewModel;

            if (chooseBatchWindowViewModel.ChooseBatchProductCollection.Count == 1)
                Close();
            else
            {
                Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
                {
                    if (notificationMessage.Notification == "CloseWindow" && notificationMessage.Sender is ChooseBatchWindowViewModel)
                        Close();
                });
                ShowDialog();
            }
        }

        private void ChooseBatchWindow_OnClosed(object sender, EventArgs e)
        {
            Messenger.Default.Unregister(this);
        }
    }
}
