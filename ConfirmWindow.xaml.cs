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
using His_Pos.Class;

namespace His_Pos
{
    /// <summary>
    /// ConfirmWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public bool Confirm = false;
        public ConfirmWindow()
        {
            InitializeComponent();
        }

        public ConfirmWindow(string message, MessageType type)
        {
            InitializeComponent();

            InitBtnUi(type);

            Message.Content = message;
        }

        private void InitBtnUi(MessageType type)
        {
            switch (type)
            {
                case MessageType.ERROR:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\DeleteDot.png", UriKind.Relative));
                    Icon.Visibility = Visibility.Visible;
                    CancelBtn.Background = Brushes.IndianRed;
                    ConfirmBtn.Background = Brushes.DimGray;
                    break;
                case MessageType.SUCCESS:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Success.png", UriKind.Relative));
                    Icon.Visibility = Visibility.Visible;
                    CancelBtn.Background = Brushes.DimGray;
                    ConfirmBtn.Background = Brushes.IndianRed;
                    break;
                case MessageType.WARNING:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Error.png", UriKind.Relative));
                    Icon.Visibility = Visibility.Visible;
                    CancelBtn.Background = Brushes.DimGray;
                    ConfirmBtn.Background = Brushes.IndianRed;
                    break;
                case MessageType.ONLYMESSAGE:
                    Icon.Visibility = Visibility.Collapsed;
                    CancelBtn.Background = Brushes.DimGray;
                    ConfirmBtn.Background = Brushes.IndianRed;
                    break;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            Confirm = true;
            this.Close();
        }
    }
}
