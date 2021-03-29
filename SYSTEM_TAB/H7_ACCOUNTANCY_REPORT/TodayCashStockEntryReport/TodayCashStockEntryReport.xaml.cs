using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.TodayCashStockEntryReport.TodayEntryDetailControl;
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
        public Visibility test;



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
            /*btnTrade.Background = Brushes.LightGray;
            btnTrade.Foreground = Brushes.White;
            btnMed.Foreground = Brushes.LightGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.LightGray;
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
            btnTrade.Foreground = Brushes.LightGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.LightGray;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.LightGray;
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

        private void StackPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void OTCAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.LightGray;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void OTCIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.LightGray;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void OTCCost_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.LightGray;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void OTCChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.LightGray;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void OTCStock_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.LightGray;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;

        }

        private void OTCStockChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.LightGray;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;

        }

        private void OTCTicket_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void OTCTicketDis_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.LightGray;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;

        }

        private void OTCRewardEmp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.LightGray;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;

        }

        private void PREAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.LightGray;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void PREIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.LightGray;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void PRECost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.LightGray;
            PREChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void PREChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.LightGray;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void PREStockChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.LightGray;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void COOPAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.LightGray;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void COOPIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.LightGray;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void COOPCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.LightGray;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void COOPChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.LightGray;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SlowAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.LightGray;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SLOWIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.LightGray;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SLOWCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.LightGray;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SLOWChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.LightGray;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
             COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void NORMALAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent; 
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
               COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.LightGray;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void NORMALIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.LightGray;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void NORMALCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.LightGray;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void NORMALChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.LightGray;
        }

        private void SELFAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.LightGray;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SELFIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.LightGray;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SELFCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.LightGray;
            SELFChange.Background = Brushes.Transparent;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }

        private void SELFChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OTCAll.Background = Brushes.Transparent;
            OTCChange.Background = Brushes.Transparent;
            OTCCost.Background = Brushes.Transparent;
            OTCIncome.Background = Brushes.Transparent;
            OTCRewardEmp.Background = Brushes.Transparent;
            OTCStockChange.Background = Brushes.Transparent;
            OTCTicket.Background = Brushes.Transparent;
            OTCTicketDis.Background = Brushes.Transparent;
            PREAll.Background = Brushes.Transparent;
            PREIncome.Background = Brushes.Transparent;
            PRECost.Background = Brushes.Transparent;
            PREChange.Background = Brushes.Transparent;
            PREStockChange.Background = Brushes.Transparent;
            SLOWAll.Background = Brushes.Transparent;
            SLOWIncome.Background = Brushes.Transparent;
            SLOWCost.Background = Brushes.Transparent;
            SLOWChange.Background = Brushes.Transparent;
            SELFAll.Background = Brushes.Transparent;
            SELFIncome.Background = Brushes.Transparent;
            SELFCost.Background = Brushes.Transparent;
            SELFChange.Background = Brushes.LightGray;
            COOPAll.Background = Brushes.Transparent;
            COOPIncome.Background = Brushes.Transparent;
            COOPCost.Background = Brushes.Transparent;
            COOPChange.Background = Brushes.Transparent;
            NORMALAll.Background = Brushes.Transparent;
            NORMALIncome.Background = Brushes.Transparent;
            NORMALCost.Background = Brushes.Transparent;
            NORMALChange.Background = Brushes.Transparent;
        }
    }
}