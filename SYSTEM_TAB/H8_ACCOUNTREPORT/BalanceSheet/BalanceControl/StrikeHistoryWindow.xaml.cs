using System.Windows;

namespace His_Pos.SYSTEM_TAB.H8_ACCOUNTREPORT.BalanceSheet.BalanceControl
{
    /// <summary>
    /// StrikeHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StrikeHistoryWindow : Window
    {
        public StrikeHistoryWindow()
        {
            InitializeComponent();
            DataContext = new StrikeHistoryViewModel();
        }
    }
}