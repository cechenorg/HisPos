using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using DataGridCell = System.Windows.Controls.DataGridCell;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.PurchaseDataGridControl
{
    /// <summary>
    /// PurchaseNormalProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseNormalProcessingControl : UserControl
    {
        public PurchaseNormalProcessingControl()
        {
            InitializeComponent();
        }

        #region ----- Define Functions -----

        #region ///// Enter MoveNext Functions /////

        private void MaskedTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            Xceed.Wpf.Toolkit.MaskedTextBox maskedTextBox = sender as Xceed.Wpf.Toolkit.MaskedTextBox;

            if (maskedTextBox is null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.SelectedIndex], ProductDataGrid.Columns[10]);

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if (firstChild is TextBox)
                    firstChild.Focus();
            }
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                MoveFocusNext(textBox);
            }
            else if (textBox.Name.Equals("ProductPriceTextbox") && textBox.IsReadOnly)
                MessageWindow.ShowMessage($"欲編輯 {(ProductDataGrid.SelectedItem as Product).ID} 單價 請先將小計歸零!", MessageType.WARNING);
            else if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                textBox.CaretIndex++;
            }
        }

        private void MoveFocusNext(object sender)
        {
            (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (Keyboard.FocusedElement is Button)
                (Keyboard.FocusedElement as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (ProductDataGrid.CurrentCell.Column is null) return;

            var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);

            if (focusedCell is null) return;

            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                    if (child is StackPanel)
                    {
                        StackPanel stackPanel = child as StackPanel;
                        if (stackPanel.Tag != null && stackPanel.Tag.ToString().Equals("NotSkip"))
                            break;
                    }
                    else if (!(child is Image))
                        break;
                }

                focusedCell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
            }

            UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

            if (firstChild is TextBox)
            {
                firstChild.Focus();
                FocusRow(firstChild as TextBox);
            }
            else
            {
                UIElement secondChild = (UIElement)VisualTreeHelper.GetChild(firstChild, 0);

                secondChild.Focus();
                FocusRow(secondChild as TextBox);
            }
        }

        private void ProductSubTotalTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                int index = ProductDataGrid.Items.IndexOf(ProductDataGrid.CurrentCell.Item);

                if (ProductDataGrid.Items.Count == index + 1)
                    MoveFocusNext(textBox);
                else
                {
                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index + 1], ProductDataGrid.Columns[3]);

                    ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                    var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                    UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                    if (firstChild is TextBox)
                        firstChild.Focus();
                }
            }
            else if (textBox.Name.Equals("ProductSubTotalTextbox") && textBox.IsReadOnly)
                MessageWindow.ShowMessage($"欲編輯 {(ProductDataGrid.SelectedItem as Product).ID} 小計 請先將單價歸零!", MessageType.WARNING);
            else if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                textBox.CaretIndex++;
            }
        }

        #endregion ///// Enter MoveNext Functions /////

        #region ///// Focus Functions /////

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
            FocusRow(textBox);
        }

        private void UIElement_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            e.Handled = true;
            textBox.Focus();
        }

        private void FocusRow(TextBox textBox)
        {
            List<TextBox> textBoxs = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, textBox.Name, ref textBoxs);

            int index = textBoxs.IndexOf(textBox);

            ProductDataGrid.SelectedItem = (ProductDataGrid.Items[index] as Product);
        }

        #endregion ///// Focus Functions /////

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is PurchaseProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((PurchaseProduct)cell.DataContext).ID, ((PurchaseProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }

        #endregion ----- Define Functions -----

        private void Amount_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void ProductSubTotalTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
            }
        }

        private void tgPay_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)tgPay.IsChecked)
            {
                lbCashPay.Foreground = Brushes.Black;
                lbNormalPay.Foreground = Brushes.DimGray;
            }
            else
            {
                lbCashPay.Foreground = Brushes.DimGray;
                lbNormalPay.Foreground = Brushes.Black;
            }
        }

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
                else
                {
                    if (firstChild is StackPanel t)
                    {
                        ((TextBox)t.Children[0]).Focus();
                        ((TextBox)t.Children[0]).SelectAll();
                    }
                }
            }
        }
    }
}