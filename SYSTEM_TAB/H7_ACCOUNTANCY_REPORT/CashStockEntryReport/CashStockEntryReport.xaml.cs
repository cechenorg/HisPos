using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.CashStockEntryReport
{
    /// <summary>
    /// CashStockEntryReport.xaml 的互動邏輯
    /// </summary>
    public partial class CashStockEntryReport : UserControl
    {
        public CashStockEntryReport()
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
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DimGray;
            Med.Visibility = Visibility.Visible;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Collapsed;
            MedCoopPreBack.Background = Brushes.LightGray;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
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
            OTCTradeBack.Background = Brushes.LightGray;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
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
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCash_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.LightGray;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedStock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.LightGray;
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCTrade_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.LightGray;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.LightGray;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCStock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.LightGray;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCReward_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.LightGray;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCFee_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.LightGray;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void OTCTicket_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.LightGray;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCoopPreBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.LightGray;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedSelfPreBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.LightGray;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCostCoopBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.LightGray;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCostSelfBack_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.LightGray;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCashNotCoop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.LightGray;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCashCoop_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.LightGray;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.LightGray;
            MedCoopChange.Background = Brushes.Transparent;
        }

        private void MedCoopChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCTradeBack.Background = Brushes.GhostWhite;
            OTCCostBack.Background = Brushes.GhostWhite;
            OTCStockBack.Background = Brushes.GhostWhite;
            OTCRewardBack.Background = Brushes.GhostWhite;
            OTCFeeBack.Background = Brushes.GhostWhite;
            OTCTicketBack.Background = Brushes.GhostWhite;
            MedCoopPreBack.Background = Brushes.GhostWhite;
            MedSelfPreBack.Background = Brushes.GhostWhite;
            MedCashBack.Background = Brushes.GhostWhite;
            MedCostCoopBack.Background = Brushes.GhostWhite;
            MedCostSelfBack.Background = Brushes.GhostWhite;
            MedStockBack.Background = Brushes.GhostWhite;
            MedCashCoop.Background = Brushes.Transparent;
            MedCashNotCoop.Background = Brushes.Transparent;
            MedChange.Background = Brushes.Transparent;
            MedCoopChange.Background = Brushes.LightGray;
        }
    }
}