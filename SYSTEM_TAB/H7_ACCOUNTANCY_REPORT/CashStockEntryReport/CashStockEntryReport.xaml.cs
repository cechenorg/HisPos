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
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Visible;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Collapsed;
        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnMed.Foreground = Brushes.DimGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Visible;
            All.Visibility = Visibility.Collapsed;
        }

        private void btnAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.DimGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.DimGray;
            btnAll.Foreground = Brushes.White;
            Med.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Visible;
        }

        private void OTCMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnMed.Foreground = Brushes.DimGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Visible;
            All.Visibility = Visibility.Collapsed;
        }

        private void MedMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Visible;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Collapsed;
        }
    }
}
