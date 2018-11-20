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

namespace His_Pos.Service
{
    /// <summary>
    /// YesNoMessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class YesNoMessageWindow : Window
    {
        public YesNoMessageWindow(string message)
        {
            InitializeComponent();
            Message.Content = message;
        }

        private void Yes_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void NoButton_OnClick_ButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
