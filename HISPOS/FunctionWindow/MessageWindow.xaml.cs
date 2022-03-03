using His_Pos.NewClass;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace His_Pos.FunctionWindow
{
    /// <summary>
    /// MessageWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MessageWindow : Window
    {
        private MessageWindow(string message, MessageType type)
        {
            InitializeComponent();
            switch (type)
            {
                case MessageType.ERROR:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Error.png", UriKind.Relative));
                    break;

                case MessageType.SUCCESS:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Success.png", UriKind.Relative));
                    break;

                case MessageType.WARNING:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Warning.png", UriKind.Relative));
                    break;

                case MessageType.TIPS:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Tips.png", UriKind.Relative));
                    break;
            }
            Message.Text = message;

            OkButton.Focus();
        }

        public static void ShowMessage(string message, MessageType type)
        {
            MessageWindow messageWindow = new MessageWindow(message, type);
            messageWindow.ShowDialog();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Close();
            }
        }
    }
}