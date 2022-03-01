using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductTypeResultControl
{
    /// <summary>
    /// MixedResultControl.xaml 的互動邏輯
    /// </summary>
    public partial class MixedResultControl : UserControl
    {
        public MixedResultControl()
        {
            InitializeComponent();
        }

        private void DataGridRow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is null) return;

            DataGridRow row = sender as DataGridRow;

            ProductDataGrid.SelectedItem = row.Item;
        }

        private void DataGridRow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ProductManageStruct)row.Item).ID, ((ProductManageStruct)row.Item).WareHouseID.ToString() }, "ShowProductDetail"));
        }
    }
}