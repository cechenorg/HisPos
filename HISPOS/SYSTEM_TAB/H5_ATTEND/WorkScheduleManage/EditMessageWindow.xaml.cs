using System.Windows;

namespace His_Pos.SYSTEM_TAB.H5_ATTEND.WorkScheduleManage
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