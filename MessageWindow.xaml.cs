using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
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

        public MessageWindow(string message, MessageType type,bool buttonFocus = false)
        {
            InitializeComponent();
            switch (type) {
                case MessageType.ERROR:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Error.png", UriKind.Relative));
                    break;
                case MessageType.SUCCESS:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Success.png", UriKind.Relative));
                    break;
                case MessageType.WARNING:
                    Icon.Source = new BitmapImage(new Uri(@"..\Images\Error.png", UriKind.Relative));
                    break;
            }
            Message.Content = message;
            if(buttonFocus)
                OkButton.Focus();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void SetLabelFontSize(double size)
        {
            Message.FontSize = size;
        }
        public void SetLabelContentAlignment(HorizontalAlignment alignment)
        {
            Message.HorizontalContentAlignment = alignment;
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {

                Close();
            }
        }
    }
}
