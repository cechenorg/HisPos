using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductGroupSetting;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail.SharedControl.GroupControl
{
    /// <summary>
    /// GroupInventoryControl.xaml 的互動邏輯
    /// </summary>
    public partial class GroupInventoryControl : UserControl
    {
        public GroupInventoryControl()
        {
            InitializeComponent();
        }

        private void Product_Click(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (!(row?.DataContext is ProductGroupSetting)) return;

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ProductGroupSetting)row.Item).ID, ((ProductGroupSetting)row.Item).WareHouseID }, "ShowProductDetail"));
        }
    }
}