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

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport {
    /// <summary>
    /// CashStockEntryReport.xaml 的互動邏輯
    /// </summary>
    public partial class CashStockEntryReport : UserControl {
        public CashStockEntryReport() {
            InitializeComponent();
            StartDate.Focus();
            StartDate.SelectionStart = 0;
        }

        private void StartDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EndDate.Focus();
                EndDate.SelectionStart = 0;
            }
        }

        private void EndDate_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton.Focus();
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }

        }

        private void btnMed_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            StockOTC.Visibility = Visibility.Collapsed;
            TradeProfit.Visibility = Visibility.Collapsed;
            CashFlow.Visibility = Visibility.Visible;
            PrescriptionProfit.Visibility = Visibility.Visible;
            StockMed.Visibility = Visibility.Visible;
        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnMed.Background = Brushes.Transparent;
            btnMed.Foreground = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnTrade.Background = Brushes.DimGray;
            StockOTC.Visibility = Visibility.Visible;
            TradeProfit.Visibility = Visibility.Visible;
            CashFlow.Visibility = Visibility.Collapsed;
            PrescriptionProfit.Visibility = Visibility.Collapsed;
            StockMed.Visibility = Visibility.Collapsed;
        }
    }
}
