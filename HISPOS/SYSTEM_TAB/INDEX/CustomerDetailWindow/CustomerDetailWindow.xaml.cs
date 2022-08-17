using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.V)
            {
                e.Handled = true;
            }
        }
    }
}