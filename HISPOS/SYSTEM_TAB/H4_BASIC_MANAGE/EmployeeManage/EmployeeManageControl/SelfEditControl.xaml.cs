using His_Pos.Service;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace His_Pos.SYSTEM_TAB.H4_BASIC_MANAGE.EmployeeManage.EmployeeManageControl
{
    /// <summary>
    /// SelfEditControl.xaml 的互動邏輯
    /// </summary>
    public partial class SelfEditControl : UserControl
    {
        public SelfEditControl()
        {
            InitializeComponent();
        }

        private void DateMaskedTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is MaskedTextBox t && e.Key == Key.Enter)
                t.Text = DateTimeExtensions.ConvertDateStringToTaiwanCalendar(t.Text);
        }
    }
}