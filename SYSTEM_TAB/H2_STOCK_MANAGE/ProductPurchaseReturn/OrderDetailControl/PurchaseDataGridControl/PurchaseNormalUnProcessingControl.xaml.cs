using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.Product;
using His_Pos.Service;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.OrderDetailControl.PurchaseDataGridControl
{
    /// <summary>
    /// PurchaseNormalUnProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class PurchaseNormalUnProcessingControl : UserControl
    {
        public PurchaseNormalUnProcessingControl()
        {
            InitializeComponent();
        }

        #region ----- Define Functions -----

        #region ///// Enter MoveNext Functions /////
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

                    (DataContext as ProductPurchaseReturnViewModel).AddProductByInputCommand.Execute(textBox.Text);

                    textBox.Text = "";

                    if (ProductDataGrid.Items.Count != oldCount)
                        ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[ProductDataGrid.Items.Count - 2], ProductDataGrid.Columns[3]);
                }
                else if (ProductDataGrid.CurrentCell.Item is Product)
                {
                    if (!(ProductDataGrid.CurrentCell.Item as Product).ID.Equals(textBox.Text))
                        (DataContext as ProductPurchaseReturnViewModel).AddProductByInputCommand.Execute(textBox.Text);

                    List<TextBox> textBoxs = new List<TextBox>();
                    NewFunction.FindChildGroup(ProductDataGrid, "ProductIDTextbox", ref textBoxs);

                    int index = textBoxs.IndexOf(sender as TextBox);

                    if (!(ProductDataGrid.Items[index] as Product).ID.Equals(textBox.Text))
                        textBox.Text = (ProductDataGrid.Items[index] as Product).ID;

                    ProductDataGrid.CurrentCell = new DataGridCellInfo(ProductDataGrid.Items[index], ProductDataGrid.Columns[3]);
                }

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
            else if (textBox.Name.Equals("ProductSubTotalTextbox") && textBox.IsReadOnly)
                MessageWindow.ShowMessage($"欲編輯 {(ProductDataGrid.SelectedItem as Product).ID} 小計 請先將單價歸零!", MessageType.WARNING);
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
        #endregion

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
        #endregion
        
        #endregion
    }
}
