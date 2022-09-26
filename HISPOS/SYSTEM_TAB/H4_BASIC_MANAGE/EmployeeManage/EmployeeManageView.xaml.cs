using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage
{
    /// <summary>
    /// EmployeeManageView.xaml 的互動邏輯
    /// </summary>
    public partial class EmployeeManageView : UserControl
    {
        public EmployeeManageView()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.V)
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}