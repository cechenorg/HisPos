using System.Windows;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerDetailWindow
{
    /// <summary>
    /// CustomerDetailWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerDetailWindow : Window
    {
        public CustomerDetailWindow(int cusID)
        {
            InitializeComponent();
            CustomerDetailWindowViewModel customerDetailWindowViewModel = new CustomerDetailWindowViewModel(cusID);
            DataContext = customerDetailWindowViewModel;
            ShowDialog();
        }
    }
}