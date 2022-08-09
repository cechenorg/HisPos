using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Product.PurchaseReturn;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn.NormalView.OrderDetailControl.ReturnDataGridControl
{
    /// <summary>
    /// ReturnSingdeProcessingControl.xaml 的互動邏輯
    /// </summary>
    public partial class ReturnSingdeProcessingControl : UserControl
    {
        public ReturnSingdeProcessingControl()
        {
            InitializeComponent();
        }

        private void ShowDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;

            if (!(cell?.DataContext is ReturnProduct)) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string[]>(this, new[] { ((ReturnProduct)cell.DataContext).ID, ((ReturnProduct)cell.DataContext).WareHouseID.ToString() }, "ShowProductDetail"));
        }
        private void Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter && e.Key != Key.Down && e.Key != Key.Up && e.Key != Key.Left && e.Key != Key.Right)
                return;
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
                    key = FocusNavigationDirection.Right;
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
            int runCount = 0;
            var focusedCell = ProductDataGrid.CurrentCell.Column?.GetCellContent(ProductDataGrid.CurrentCell.Item);
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
                focusedCell?.MoveFocus(new TraversalRequest(key));
                focusedCell = ProductDataGrid.CurrentCell.Column.GetCellContent(ProductDataGrid.CurrentCell.Item);
                if (runCount > 10)
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