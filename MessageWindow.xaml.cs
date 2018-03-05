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

namespace His_Pos
{
    /// <summary>
    /// MessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }

        public MessageWindow(string message, MessageType type)
        {
            InitializeComponent();

            if (type == MessageType.ERROR)
                Icon.Source = new BitmapImage(new Uri(@"..\Images\Error.png", UriKind.Relative));

            Message.Content = message;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
