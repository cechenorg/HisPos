using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private bool isWindowClosed = false;

        public ChooseBatchWindow(string productID, string wareID)
        {
            InitializeComponent();
            
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseChooseBatchWindow")
                {
                    isWindowClosed = true;
                    Close();
                }
            });

            DataContext = new ChooseBatchWindowViewModel(productID, wareID);

            if(!isWindowClosed)
                ShowDialog();
        }
    }
}
