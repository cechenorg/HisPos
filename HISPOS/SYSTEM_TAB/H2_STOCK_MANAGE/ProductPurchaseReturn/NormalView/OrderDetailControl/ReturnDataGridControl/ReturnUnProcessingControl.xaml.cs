using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.ReturnDataGridControl
{
    /// <summary>
    /// ReturnUnProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnUnProcessingControl : UserControl
    {
        public ReturnUnProcessingControl()
        {
            InitializeComponent();
        }

        #region ----- Define Functions -----

        private void InputTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();

            List<TextBox> textBoxs = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, textBox.Name, ref textBoxs);

            int index = textBoxs.IndexOf(sender as TextBox);

            ProductDataGrid.SelectedItem = (ProductDataGrid.Items[index] as Product);
        }

        private void InputTextbox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void ProductIDTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (ProductDataGrid.CurrentCell.Item.ToString().Equals("{NewItemPlaceholder}") && !textBox.Text.Equals(string.Empty))
                {
                    int oldCount = ProductDataGrid.Items.Count;

                    (DataContext as NormalViewModel).AddProductByInputCommand.Execute(textBox.Text);

                    textBox.Text = "";

                    if (ProductDataGrid.Items.Count != oldCount)
                        ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.Items.Count - 2], ProductDataGrid.Columns[4]);
                }
                else if (ProductDataGrid.CurrentCell.Item is Product)
                {
                    if (!(ProductDataGrid.CurrentCell.Item as Product).ID.Equals(textBox.Text))
                        (DataContext as NormalViewModel).AddProductByInputCommand.Execute(textBox.Text);

                    List<TextBox> textBoxs = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox", ref textBoxs);

                    int index = textBoxs.IndexOf(sender as TextBox);

                    if (!(ProductDataGrid.Items[index] as Product).ID.Equals(textBox.Text))
                        textBox.Text = (ProductDataGrid.Items[index] as Product).ID;

                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index], ProductDataGrid.Columns[4]);
                }

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                //var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                //UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                //if (firstChild is TextBox)
                //    firstChild.Focus();
            }
        }

        private void OrderAmountTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                int index = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentCell.Item);

                ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index + 1], ProductDataGrid.Columns[1]);

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if (firstChild is TextBox)
                    firstChild.Focus();
            }
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is ReturnProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ReturnProduct)cell.DataContext).ID, ((ReturnProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }

        #endregion ----- Define Functions -----

        private void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                var uie = e.OriginalSource as UIElement;
                uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                var focusedCell = ProductDataGrid.CurrentCell.Column?.GetCellContent(ProductDataGrid.CurrentCell.Item);
                var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                if ((firstChild is TextBox || firstChild is TextBlock) && firstChild.Focusable)
                {
                    firstChild.Focus();
                    if (firstChild is TextBox t)
                        t.SelectAll();
                }
            }
        }
    }
}