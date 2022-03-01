using GalaSoft.MvvmLight.Messaging;
using His_Pos.Database;
using His_Pos.NewClass.Report.PrescriptionDetailReport.PrescriptionDetailMedicineRepot;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductManagement.ProductDetail;
using His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.TodayCashStockEntryReport.TodayEntryDetailControl;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.NewTodayCashStockEntryReport
{
    /// <summary>
    /// CashStockEntryReport.xaml 的互動邏輯
    /// </summary>
    public partial class NewTodayCashStockEntryReport : UserControl
    {
        public Visibility test;
        public DataSet Ds = new DataSet();


        public NewTodayCashStockEntryReport()
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
        private void ShowProductDetail(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;

            if (row?.Item is null) return;

            ProductDetailWindow.ShowProductDetailWindow();

            Messenger.Default.Send(new NotificationMessage<string>(this, ((PrescriptionDetailMedicineRepot)row.Item).Id, "ShowProductDetail"));
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
            /*btnTrade.Background = Brushes.DarkGray;
            btnTrade.Foreground = Brushes.White;
            btnMed.Foreground = Brushes.DarkGray;
            btnMed.Background = Brushes.Transparent;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DarkGray;
            Med.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Visible;
            All.Visibility = Visibility.Collapsed;
            OTCTradeBack.Background = Brushes.DarkGray;
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
            btnTrade.Foreground = Brushes.DarkGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DarkGray;
            btnAll.Background = Brushes.Transparent;
            btnAll.Foreground = Brushes.DarkGray;
            Med.Visibility = Visibility.Visible;
            OTC.Visibility = Visibility.Collapsed;
            All.Visibility = Visibility.Collapsed;
            MedPreBack.Background = Brushes.DarkGray;
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
            OTCAll.Background = Brushes.DarkGray;
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
            OTCIncome.Background = Brushes.DarkGray;
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
            OTCCost.Background = Brushes.DarkGray;
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
            OTCChange.Background = Brushes.DarkGray;
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
            OTCStockChange.Background = Brushes.DarkGray;
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
            OTCStockChange.Background = Brushes.DarkGray;
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
            OTCTicketDis.Background = Brushes.DarkGray;
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
            OTCRewardEmp.Background = Brushes.DarkGray;
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
            PREAll.Background = Brushes.DarkGray;
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
            PREIncome.Background = Brushes.DarkGray;
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
            PRECost.Background = Brushes.DarkGray;
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
            PREChange.Background = Brushes.DarkGray;
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
            PREStockChange.Background = Brushes.DarkGray;
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
            COOPAll.Background = Brushes.DarkGray;
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
            COOPIncome.Background = Brushes.DarkGray;
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
            COOPCost.Background = Brushes.DarkGray;
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
            COOPChange.Background = Brushes.DarkGray;
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
            SLOWAll.Background = Brushes.DarkGray;
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
            SLOWIncome.Background = Brushes.DarkGray;
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
            SLOWCost.Background = Brushes.DarkGray;
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
            SLOWChange.Background = Brushes.DarkGray;
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
            NORMALAll.Background = Brushes.DarkGray;
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
            NORMALIncome.Background = Brushes.DarkGray;
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
            NORMALCost.Background = Brushes.DarkGray;
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
            NORMALChange.Background = Brushes.DarkGray;
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
            SELFAll.Background = Brushes.DarkGray;
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
            SELFIncome.Background = Brushes.DarkGray;
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
            SELFCost.Background = Brushes.DarkGray;
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
            SELFChange.Background = Brushes.DarkGray;
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