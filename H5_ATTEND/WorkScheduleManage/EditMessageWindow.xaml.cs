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
using System.Windows.Shapes;

namespace His_Pos.H5_ATTEND.WorkScheduleManage
{
    /// <summary>
    /// EditMessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class EditMessageWindow : Window
    {
        public string Message { get; set; }

        public EditMessageWindow(string message)
        {
            InitializeComponent();

            Message = message;

            MessageTb.Text = message;
            MessageTb.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Message = MessageTb.Text.ToString();

            Close();
        }
    }
}
