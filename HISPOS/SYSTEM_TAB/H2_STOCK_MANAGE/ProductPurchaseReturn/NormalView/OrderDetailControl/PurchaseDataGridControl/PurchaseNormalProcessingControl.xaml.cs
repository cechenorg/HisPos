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
using Xceed.Wpf.Toolkit;
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
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right) return;
            e.Handled = true;
            if (sender is null) return;
            switch (e.Key)
            {
                case Key.Enter:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;

                case Key.Up:
                    MoveFocusNext(sender, FocusNavigationDirection.Up);
                    break;

                case Key.Down:
                    MoveFocusNext(sender, FocusNavigationDirection.Down);
                    break;

                case Key.Left:
                    MoveFocusNext(sender, FocusNavigationDirection.Left);
                    break;

                case Key.Right:
                    MoveFocusNext(sender, FocusNavigationDirection.Next);
                    break;
            }
        }
        private void MoveFocusNext(object sender, FocusNavigationDirection direction)
        {
            int i = ProductDataGrid.Columns.IndexOf(ProductDataGrid.CurrentColumn);
            List<PurchaseMedicine> detail = new List<PurchaseMedicine>();
            foreach (PurchaseMedicine item in ProductDataGrid.ItemsSource)
            {
                detail.Add(item);
            }
            int currentIndex = detail.IndexOf((PurchaseMedicine)ProductDataGrid.CurrentCell.Item);
            if(i == 5 && currentIndex == 0 && (direction == FocusNavigationDirection.Left || direction == FocusNavigationDirection.Up))
            {
                return;
            }
            if(i == ProductDataGrid.Columns.Count - 1 && currentIndex == detail.Count - 1 && (direction == FocusNavigationDirection.Right || direction == FocusNavigationDirection.Down || direction == FocusNavigationDirection.Next))
            {
                return;
            }
            switch (sender)
            {
                case null:
                    return;

                case TextBox box:
                    if (direction.Equals(FocusNavigationDirection.Next))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    else if (direction.Equals(FocusNavigationDirection.Up))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    else if (direction.Equals(FocusNavigationDirection.Left))
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                    else
                        box.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    break;
            }

            if (sender != null)
            {
                if (sender is TextBox)
                {
                    TextBox box = (TextBox)sender;
                    if (box.Parent is StackPanel)
                    {
                        var focusedPanelCell = ProductDataGrid.Columns[i+1].GetCellContent(ProductDataGrid.CurrentCell.Item);
                        switch (direction)
                        {
                            case FocusNavigationDirection.Next:
                            case FocusNavigationDirection.Right:
                                focusedPanelCell = ProductDataGrid.Columns[i + 1].GetCellContent(ProductDataGrid.CurrentCell.Item);
                                break;
                            case FocusNavigationDirection.Left:
                                focusedPanelCell = ProductDataGrid.Columns[i - 1].GetCellContent(ProductDataGrid.CurrentCell.Item);
                                break;
                            case FocusNavigationDirection.Up:
                            case FocusNavigationDirection.Down:
                                focusedPanelCell = ProductDataGrid.Columns[i].GetCellContent(ProductDataGrid.CurrentCell.Item);
                                break;
                        }

                        focusedPanelCell.MoveFocus(new TraversalRequest(direction));
                        UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedPanelCell, 0);
                        child.Focus();
                        if (child is MaskedTextBox m)
                        {
                            m.SelectAll();
                            return;
                        }
                        if (child is TextBox t)
                        {
                            t.SelectAll();
                            return;
                        }
                    }
                }
            }

            var focusedCell = ProductDataGrid.CurrentCell.Column?.GetCellContent(ProductDataGrid.CurrentCell.Item);
            int runCount = 0;
            
            if (focusedCell is null) return;
            while (true)
            {
                if (focusedCell is ContentPresenter)
                {
                    UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    while (child is ContentPresenter)
                    {
                        child = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                    }

                    if (child is TextBox || child is TextBlock)
                        break;
                    if (child is StackPanel s)
                    {
                        if (s.Children[0] is TextBox)
                        {
                            break;
                        }
                    }
                }
                i = ProductDataGrid.Columns.IndexOf(ProductDataGrid.CurrentColumn);
                if (i == 0)
                {
                    if (ProductDataGrid.CurrentCell.Item is PurchaseMedicine medicine)
                    {
                        if (currentIndex > 0 && direction == FocusNavigationDirection.Left)
                        {
                            var focusedPanelCell = ProductDataGrid.Columns[ProductDataGrid.Columns.Count - 1].GetCellContent(detail[currentIndex - 1]);
                            UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedPanelCell, 0);
                            child.Focus();
                            if (child is TextBox t)
                            {
                                t.SelectAll();
                                return;
                            }
                        }
                    }
                }
                focusedCell?.MoveFocus(new TraversalRequest(direction));
                focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                runCount++;
                if (runCount > 10)
                    break;
            }
            if(runCount < 10)
            {
                var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if ((firstChild is TextBox || firstChild is TextBlock) && firstChild.Focusable)
                {
                    firstChild.Focus();
                    if (firstChild is TextBox t)
                        t.SelectAll();
                }
                else
                {
                    if (firstChild is StackPanel s)
                    {
                        if (s.Children[0] is TextBox)
                        {
                            ((TextBox)s.Children[0]).Focus();
                            ((TextBox)s.Children[0]).SelectAll();
                        }
                    }
                }
            }
        }
    }
}