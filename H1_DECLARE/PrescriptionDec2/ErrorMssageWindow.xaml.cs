using System.Windows;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// ErrorMssageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ErrorMssageWindow : Window
    {
        public bool ErrorLogIn = false;
        public ErrorMssageWindow()
        {
            InitializeComponent();
        }

        public ErrorMssageWindow(string message)
        {
            InitializeComponent();
            Message.Content = message;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ErrorLogInClick(object sender, RoutedEventArgs e)
        {
            ErrorLogIn = true;
            Close();
        }
    }
}
