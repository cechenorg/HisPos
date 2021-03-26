using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.TodayCashStockEntryReport
{
    /// <summary>
    /// CashStockEntryReport.xaml 的互動邏輯
    /// </summary>
    public partial class TodayCashStockEntryReport : UserControl
    {
        public TodayCashStockEntryReport()
        {
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
          
        }

        private void btnTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void btnAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void OTCMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*btnTrade.Background = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnMed.Foreground = Brushes.DimGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Visible;
            All.Visibility = Visibility.Collapsed;
            OTCTradeBack.Background = Brushes.LightGray;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            MedPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;*/
        }

        private void MedMain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /*btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Visible;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Collapsed;
            MedPreBack.Background = Brushes.LightGray;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;*/
        }

        private void MedPre_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void MedCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
         
        }

        private void MedCash_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void MedStock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void OTCTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void OTCCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void OTCStock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void OTCReward_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void OTCFee_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void OTCTicket_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void MedCoopPreBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MedSelfPreBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void MedCostCoopBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MedCostSelfBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void MedCashNotCoop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void MedCashCoop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MedChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
          
        }

        private void MedCoopChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }
    }
}