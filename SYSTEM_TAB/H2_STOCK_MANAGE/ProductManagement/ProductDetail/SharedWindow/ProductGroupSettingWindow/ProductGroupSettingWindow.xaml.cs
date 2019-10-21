using GalaSoft.MvvmLight.Messaging;
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

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedWindow.ProductGroupSettingWindow {
    /// <summary>
    /// ProductGroupSettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ProductGroupSettingWindow : Window {
        public ProductGroupSettingWindow(NewClass.Product.ProductGroupSetting.ProductGroupSettings productGroupSettingCollection, string wareID, double inventory) {
            InitializeComponent();

            ProductGroupSettingWindowViewModel productGroupSettingWindowViewModel = new ProductGroupSettingWindowViewModel(productGroupSettingCollection, wareID, inventory);
            DataContext = productGroupSettingWindowViewModel;

            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseProductGroupSettingWindow"))
                    Close();
            });
        }
    }
}
