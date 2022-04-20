using System.Data; 
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
         

        private void OTCAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCAll.Background = Brushes.DarkGray; 
        }

        private void OTCIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCIncome.Background = Brushes.DarkGray; 
        }

        private void OTCCost_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCCost.Background = Brushes.DarkGray; 
        }

        private void OTCChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCChange.Background = Brushes.DarkGray; 
        }

        
        private void OTCStockChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCStockChange.Background = Brushes.DarkGray;  
        }
         
        private void OTCTicketDis_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCTicketDis.Background = Brushes.DarkGray;  
        }

        private void OTCRewardEmp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            OTCRewardEmp.Background = Brushes.DarkGray;  
        }

        private void PREAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            PREAll.Background = Brushes.DarkGray; 
        }

        private void PREIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            PREIncome.Background = Brushes.DarkGray; 
        }

        private void PRECost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            PRECost.Background = Brushes.DarkGray; 
        }

        private void PREChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            PREChange.Background = Brushes.DarkGray; 
        }

        private void PREStockChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            PREStockChange.Background = Brushes.DarkGray; 
        }

        private void COOPAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            COOPAll.Background = Brushes.DarkGray; 
        }

        private void COOPIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            COOPIncome.Background = Brushes.DarkGray; 
        }

        private void COOPCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            COOPCost.Background = Brushes.DarkGray; 
        }

        private void COOPChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            COOPChange.Background = Brushes.DarkGray; 
        }

        private void SlowAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SLOWAll.Background = Brushes.DarkGray; 
        }

        private void SLOWIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SLOWIncome.Background = Brushes.DarkGray; 
        }

        private void SLOWCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SLOWCost.Background = Brushes.DarkGray; 
        }

        private void SLOWChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SLOWChange.Background = Brushes.DarkGray; 
        }

        private void NORMALAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            NORMALAll.Background = Brushes.DarkGray; 
        }

        private void NORMALIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            NORMALIncome.Background = Brushes.DarkGray; 
        }

        private void NORMALCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            NORMALCost.Background = Brushes.DarkGray; 
        }

        private void NORMALChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            NORMALChange.Background = Brushes.DarkGray;
        }

        private void SELFAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SELFAll.Background = Brushes.DarkGray; 
        }

        private void SELFIncome_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SELFIncome.Background = Brushes.DarkGray; 
        }

        private void SELFCost_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SELFCost.Background = Brushes.DarkGray; 
        }

        private void SELFChange_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetUITransparent();
            SELFChange.Background = Brushes.DarkGray; 
        }

        private void SetUITransparent()
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
            NORMALChange.Background = Brushes.Transparent;
        }
    }
}