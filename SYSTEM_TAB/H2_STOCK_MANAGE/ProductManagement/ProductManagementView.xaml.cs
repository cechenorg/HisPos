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
using His_Pos.NewClass.Product.ProductManagement;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement
{
    /// <summary>
    /// ProductManagementView.xaml 的互動邏輯
    /// </summary>
    public partial class ProductManagementView : UserControl
    {
        public ProductManagementView()
        {
            InitializeComponent();
        }

        private void SearchTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
        }

        private void SearchTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void DataGridRow_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is null) return;

            DataGridRow row = sender as DataGridRow;

            ProductDataGrid.SelectedItem = row.Item;
        }

        private void DataGridRow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string>(this, ((ProductManageStruct)row.Item).ID, nameof(ProductManagementView)));
        }
    }
}
