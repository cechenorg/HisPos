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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductGroupSetting;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl {
    /// <summary>
    /// GroupInventoryControl.xaml 的互動邏輯
    /// </summary>
    public partial class GroupInventoryControl : UserControl {
        public GroupInventoryControl() {
            InitializeComponent();
        }

        private void Product_Click(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (!(row?.DataContext is ProductGroupSetting)) return;

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ProductGroupSetting)row.Item).ID, "0" }, "ShowProductDetail"));
        }
    }
}
