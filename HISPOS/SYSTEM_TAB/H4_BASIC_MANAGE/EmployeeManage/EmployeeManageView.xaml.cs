using System.Windows.Controls;

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

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Main.SelectedIndex = 0;
        }
    }
}