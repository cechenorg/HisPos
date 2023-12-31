﻿using GalaSoft.MvvmLight.Messaging;
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

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.ReturnDataGridControl
{
    /// <summary>
    /// ReturnNormalProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnNormalProcessingControl : UserControl
    {
        public ReturnNormalProcessingControl()
        {
            InitializeComponent();
        }

        #region ----- Define Functions -----

        #region ///// Enter MoveNext Functions /////

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
            else if (textBox.Name.Equals("ProductSubTotalTextbox") && textBox.IsReadOnly)
                MessageWindow.ShowMessage($"欲編輯 {(ProductDataGrid.SelectedItem as Product).ID} 小計 請先將單價歸零!", MessageType.WARNING);
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

            if (!(cell?.DataContext is ReturnProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ReturnProduct)cell.DataContext).ID, ((ReturnProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }

        #endregion ----- Define Functions -----

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void RealAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right) 
                return;
            int i = ProductDataGrid.Columns.IndexOf(ProductDataGrid.CurrentColumn);
            var Type = ProductDataGrid.CurrentCell.Item.GetType().Name;
            List<object> detail = new List<object>();
            foreach (var item in ProductDataGrid.ItemsSource)
            {
                detail.Add(item);
            }
            int currentIndex = 0;
            if (Type == "ReturnMedicine")
            {
                currentIndex = detail.IndexOf((ReturnMedicine)ProductDataGrid.CurrentCell.Item);
            }
            else
            {
                currentIndex = detail.IndexOf((ReturnOTC)ProductDataGrid.CurrentCell.Item);
            }
            if (i == 4 && e.Key == Key.Left)
            {
                return;
            }
            FocusNavigationDirection key = FocusNavigationDirection.Next;
            switch(e.Key)
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
            e.Handled = true;
            var uie = e.OriginalSource as UIElement;
            uie.MoveFocus(new TraversalRequest(key));
            var focusedCell = ProductDataGrid.CurrentCell.Column?.GetCellContent(ProductDataGrid.CurrentCell.Item);
            if(i == 7)
            {
                focusedCell = ProductDataGrid.Columns[5].GetCellContent(ProductDataGrid.CurrentCell.Item);
            }
            if (e.Key == Key.Right && i == 5)
            {
                focusedCell = ProductDataGrid.Columns[7].GetCellContent(ProductDataGrid.CurrentCell.Item);
            }
            if (e.Key == Key.Enter && currentIndex == ProductDataGrid.Items.Count - 1 && i == 7)
                return;
            try
            {
                var firstChild = (UIElement)VisualTreeHelper.GetChild(focusedCell, 0);
                if ((firstChild is TextBox || firstChild is TextBlock) && firstChild.Focusable)
                {
                    firstChild.Focus();
                    if (firstChild is TextBox t)
                        t.SelectAll();
                }
            }
            catch
            { }
        }

        private void ProductSubTotalTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "")
            { tb.Text = "0"; }
        }
    }
}