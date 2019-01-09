using System.Windows;

namespace His_Pos.FunctionWindow
{
    /// <summary>
    /// YesNoMessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class YesNoMessageWindow : Window
    {
        public string Message { get; set; }
        public YesNoMessageWindow(string message,string title)
        {
            InitializeComponent();
            Message = message;
            TitleLabel.Content = title;
            DataContext = this;
            YesButton.Focus();
        }

        private void Yes_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void NoButton_OnClick_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
