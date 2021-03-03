using System.Windows;

namespace His_Pos.SYSTEM_TAB.P1_TRANSACTION.ProductTransaction.CustomerDataControl
{
    /// <summary>
    /// GetCustomerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class GetCustomerWindow : Window
    {
        #region ----- Define Variables -----

        public string SearchString { get; set; }

        #endregion ----- Define Variables -----

        public GetCustomerWindow(string searchString)
        {
            InitializeComponent();

            SearchString = searchString;
        }
    }
}