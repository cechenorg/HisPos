using System.Windows;
using System.Windows.Media;
using UserControl = System.Windows.Controls.UserControl;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.EntrySerach
{
    /// <summary>
    /// EntrySearchView.xaml 的互動邏輯
    /// </summary>
    public partial class EntrySearchView : UserControl
    {
        public EntrySearchView()
        {
            InitializeComponent();
        }

        private void btnTrade_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnMed.Background = Brushes.Transparent;
            btnMed.Foreground = Brushes.DimGray;
            btnTrade.Foreground = Brushes.White;
            btnTrade.Background = Brushes.DimGray;
            OTC.Visibility = Visibility.Visible;
            OTCDG.Visibility = Visibility.Visible;
            OTCPrint.Visibility = Visibility.Visible;
            Print.Visibility = Visibility.Collapsed;
            Med.Visibility = Visibility.Collapsed;
            MedDG.Visibility = Visibility.Collapsed;
        }

        private void btnMed_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnTrade.Background = Brushes.Transparent;
            btnTrade.Foreground = Brushes.DimGray;
            btnMed.Foreground = Brushes.White;
            btnMed.Background = Brushes.DimGray;
            Med.Visibility = Visibility.Visible;
            MedDG.Visibility = Visibility.Visible;
            OTCDG.Visibility = Visibility.Collapsed;
            OTCPrint.Visibility = Visibility.Collapsed;
            OTC.Visibility = Visibility.Collapsed;
            Print.Visibility = Visibility.Visible;
        }
    }
}