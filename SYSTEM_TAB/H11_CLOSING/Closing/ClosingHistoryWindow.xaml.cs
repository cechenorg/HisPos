using ClosingHistoryViewModelHis_Pos.SYSTEM_TAB.H11_CLOSING.Closing;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H11_CLOSING.Closing
{
    /// <summary>
    /// StrikeHistoryWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ClosingHistoryWindow : Window
    {
        public ClosingHistoryWindow()
        {
            InitializeComponent();
            DataContext = new ClosingHistoryViewModel();
        }
    }
}