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
                        ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.Items.Count - 2], ProductDataGrid.Columns[5]);
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

                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index], ProductDataGrid.Columns[5]);
                }

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if (firstChild is TextBox)
                    firstChild.Focus();
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

                ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index + 1], ProductDataGrid.Columns[2]);

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
            if (e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right)
                return;
            int i = ProductDataGrid.Columns.IndexOf(ProductDataGrid.CurrentColumn);

            FocusNavigationDirection key = FocusNavigationDirection.Next;
            switch (e.Key)
            {
                case Key.Enter:
                    key = FocusNavigationDirection.Next;
                    break;
                case Key.Left:
                    key = FocusNavigationDirection.Left;
                    break;
                case Key.Right:
                    key = FocusNavigationDirection.Next;
                    break;
                case Key.Up:
                    key = FocusNavigationDirection.Up;
                    break;
                case Key.Down:
                    key = FocusNavigationDirection.Down;
                    break;
            }
            if (i == 2 && (e.Key == Key.Left || e.Key == Key.Up))
            {
                if (sender != null)
                {
                    if (sender is TextBox)
                    {
                        TextBox box = (TextBox)sender;
                        if (i == 2)
                        {
                            if (key == FocusNavigationDirection.Left)
                            {
                                var type = ProductDataGrid.CurrentCell.Item.ToString();
                                if (type.Contains("ReturnMedicine"))
                                {
                                    if (ProductDataGrid.CurrentCell.Item is ReturnMedicine medicine)
                                    {
                                        var detail = new List<ReturnMedicine>();
                                        foreach (ReturnMedicine item in ProductDataGrid.ItemsSource)
                                        {
                                            detail.Add(item);
                                        }
                                        int currentIndex = detail.IndexOf(medicine);
                                        if (currentIndex > 0)
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
                                        else
                                        {
                                            var focusedPanelCell = ProductDataGrid.Columns[2].GetCellContent(detail[0]);
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
                                if (type.Contains("ReturnOTC"))
                                {
                                    var detail = new List<ReturnOTC>();
                                    foreach (ReturnOTC item in ProductDataGrid.ItemsSource)
                                    {
                                        detail.Add(item);
                                    }
                                    if (ProductDataGrid.CurrentCell.Item is ReturnOTC medicine)
                                    {
                                        int currentIndex = detail.IndexOf(medicine);
                                        if (currentIndex > 0)
                                        {
                                            var focusedPanelCell = ProductDataGrid.Columns[ProductDataGrid.CurrentColumn.DisplayIndex - 1].GetCellContent(detail[currentIndex - 1]);
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
                            }
                        }
                    }
                }
            }
            e.Handled = true;
            var uie = e.OriginalSource as UIElement;
            uie.MoveFocus(new TraversalRequest(key));
            var focusedCell = ProductDataGrid.CurrentCell.Column?.GetCellContent(ProductDataGrid.CurrentCell.Item);
            int runCount = 0;
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
                    if (child is CheckBox c)
                    {
                        if (c is CheckBox)
                        {
                            if(ProductDataGrid.CurrentCell.Item is ReturnMedicine medicine)
                            {
                                var detail = new List<ReturnMedicine>();
                                foreach (ReturnMedicine item in ProductDataGrid.ItemsSource)
                                {
                                    detail.Add(item);
                                }
                                int currentIndex = detail.IndexOf(medicine);
                                focusedCell = ProductDataGrid.Columns[ProductDataGrid.CurrentColumn.DisplayIndex + 1].GetCellContent(detail[currentIndex]);
                                break;
                            }
                            if (ProductDataGrid.CurrentCell.Item is ReturnOTC otc)
                            {
                                var detail = new List<ReturnOTC>();
                                foreach (ReturnOTC item in ProductDataGrid.ItemsSource)
                                {
                                    detail.Add(item);
                                }
                                int currentIndex = detail.IndexOf(otc);
                                focusedCell = ProductDataGrid.Columns[ProductDataGrid.Columns.Count + 1].GetCellContent(detail[currentIndex + 1]);
                                break;
                            }
                        }
                    }
                }
                focusedCell.MoveFocus(new TraversalRequest(key));
                focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                if(runCount > 10)
                {
                    return;
                }
                runCount++;
            }
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