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
using His_Pos.Class;

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
