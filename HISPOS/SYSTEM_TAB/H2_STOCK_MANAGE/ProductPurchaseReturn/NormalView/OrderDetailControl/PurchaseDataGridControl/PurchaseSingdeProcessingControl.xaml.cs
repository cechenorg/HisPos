using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;
using His_Pos.NewClass.Product;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.NewClass.StoreOrder;
using His_Pos.Service;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.PurchaseDataGridControl
{
    /// <summary>
    /// PurchaseSingdeProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseSingdeProcessingControl : UserControl
    {
        public PurchaseSingdeProcessingControl()
        {
            InitializeComponent();
        }
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
        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is PurchaseProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((PurchaseProduct)cell.DataContext).ID, ((PurchaseProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }
        private int GetRowIndex(MouseButtonEventArgs e)
        {
            DataGridRow dgr = null;
            DependencyObject visParent = VisualTreeHelper.GetParent(e.OriginalSource as FrameworkElement);
            while (dgr == null && visParent != null)
            {
                dgr = visParent as DataGridRow;
                visParent = VisualTreeHelper.GetParent(visParent);
            }
            if (dgr == null) { return -1; }
            int rowIdx = dgr.GetIndex();
            return rowIdx;
        }
        private void HISTORYDG_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           // StoreOrderHistory row = (StoreOrderHistory)HISTORYDG.SelectedItems[0];



           // string proID = row.ID.ToString();


            //    ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

           //    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, proID, ""));

            }

        //private void DataGrid_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    StoreOrderHistory row = (StoreOrderHistory)HISTORYDG.SelectedItems[0];



        //    string proID = row.ID.ToString();


        //    ProductPurchaseRecordViewModel viewModel = (App.Current.Resources["Locator"] as ViewModelLocator).ProductPurchaseRecord;

        //    Messenger.Default.Send(new NotificationMessage<string>(this, viewModel, proID, ""));
        //}
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
                        ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.Items.Count - 2], ProductDataGrid.Columns[7]);
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

                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index], ProductDataGrid.Columns[7]);
                }

                ProductDataGrid.SelectedItem = ProductDataGrid.CurrentCell.Item;

                var focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                UIElement firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);

                if (firstChild is TextBox)
                    firstChild.Focus();
            }
        }

        private void FocusRow(TextBox textBox)
        {
            List<TextBox> textBoxs = new List<TextBox>();
            NewFunction.FindChildGroup(ProductDataGrid, textBox.Name, ref textBoxs);

            int index = textBoxs.IndexOf(textBox);

            ProductDataGrid.SelectedItem = ProductDataGrid.Items[index/2] as Product;
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

                    if (!(child is Image))
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
        }
        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
            FocusRow(textBox);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrEmpty(textBox.Text)) 
            {
                textBox.Text = "0";
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox is null) return;

            textBox.SelectAll();
            FocusRow(textBox);
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
                    int i = ProductDataGrid.Columns.IndexOf(ProductDataGrid.CurrentColumn);
                    if (box.Parent is StackPanel)
                    {
                        var focusedPanelCell = ProductDataGrid.Columns[i].GetCellContent(ProductDataGrid.CurrentCell.Item);
                        switch (direction)
                        {
                            case FocusNavigationDirection.Next:
                            case FocusNavigationDirection.Right:
                                focusedPanelCell = ProductDataGrid.Columns[i + 1].GetCellContent(ProductDataGrid.CurrentCell.Item);
                                break;
                            case FocusNavigationDirection.Left:
                            case FocusNavigationDirection.Up:
                            case FocusNavigationDirection.Down:
                                focusedPanelCell = ProductDataGrid.Columns[i].GetCellContent(ProductDataGrid.CurrentCell.Item);
                                break;
                        }
                        
                        focusedPanelCell.MoveFocus(new TraversalRequest(direction));
                        UIElement child = (UIElement)VisualTreeHelper.GetChild(focusedPanelCell, 0);
                        child.Focus();
                        if(child is MaskedTextBox m)
                        {
                            m.SelectAll();
                            return;
                        }
                        if(child is TextBox t)
                        {
                            t.SelectAll();
                            return;
                        }
                    }
                    if (i == 1)
                    {
                        if(direction == FocusNavigationDirection.Left)
                        {
                            var type = ProductDataGrid.CurrentCell.Item.ToString();
                            if (type.Contains("PurchaseMedicine"))
                            {
                                List<PurchaseMedicine> detail = new List<PurchaseMedicine>();
                                foreach (PurchaseMedicine item in ProductDataGrid.ItemsSource)
                                {
                                    detail.Add(item);
                                }
                                if (ProductDataGrid.CurrentCell.Item is PurchaseMedicine medicine)
                                {
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
                                }
                            }
                            if(type.Contains("PurchaseOTC"))
                            {
                                var detail = new List<PurchaseOTC>();
                                foreach (PurchaseOTC item in ProductDataGrid.ItemsSource)
                                {
                                    detail.Add(item);
                                }
                                if (ProductDataGrid.CurrentCell.Item is PurchaseOTC medicine)
                                {
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
                                }
                            }
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
                        if(s.Children[0] is TextBox)
                        {
                            break;
                        }
                    }
                }
                
                focusedCell?.MoveFocus(new TraversalRequest(direction));
                focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                runCount++;
                if (runCount > 10)
                    break;
            }

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
                    if(s.Children[0] is TextBox)
                    {
                        ((TextBox)s.Children[0]).Focus();
                        ((TextBox)s.Children[0]).SelectAll();
                    }
                }
            }
        }
    }
}
