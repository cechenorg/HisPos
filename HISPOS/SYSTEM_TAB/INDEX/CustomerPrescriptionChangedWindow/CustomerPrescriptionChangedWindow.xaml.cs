using System.Windows;

namespace His_Pos.SYSTEM_TAB.INDEX.CustomerPrescriptionChangedWindow
{
    /// <summary>
    /// CustomerPrescriptionChangedWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CustomerPrescriptionChangedWindow : Window
    {
        public CustomerPrescriptionChangedWindow(int cusID)
        {
            InitializeComponent();
            CustomerPrescriptionChangedViewModel customerPrescriptionChangedViewModel = new CustomerPrescriptionChangedViewModel(cusID);
            DataContext = customerPrescriptionChangedViewModel;
            ShowDialog();
        }
    }
}