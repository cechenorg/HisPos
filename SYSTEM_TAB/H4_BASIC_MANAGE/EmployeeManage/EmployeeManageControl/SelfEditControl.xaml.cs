using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Service;
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
