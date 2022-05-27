using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.Class;
using His_Pos.FunctionWindow;
using His_Pos.NewClass.StoreOrder;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows;
using His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseReturn;
using System.Windows.Controls;
using System;

namespace His_Pos.SYSTEM_TAB.H2_STOCK_MANAGE.ProductPurchaseRecord
{
    public partial class ScrapOrderWindow : Window
    {
        public ScrapOrderWindow()
        {
            InitializeComponent();
            DataContext = new ScrapOrderWindowViewModel();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "YesAction":
                        if(radio1.IsChecked != true && radio2.IsChecked != true && radio3.IsChecked != true && radio4.IsChecked != true)//必須選擇其中一項
                        {
                            MessageWindow.ShowMessage("請選擇作廢原因", MessageType.ERROR);
                            return;
                        }
                        if (radio4.IsChecked == true && other.Text == string.Empty)//選擇其他必須填寫原因
                        {
                            MessageWindow.ShowMessage("請填寫其他", MessageType.ERROR);
                            return;
                        }
                        ((ScrapOrderWindowViewModel)DataContext).Content = radioContent;
                        DialogResult = true;
                        Close();
                        break;

                    case "NoAction":
                        DialogResult = false;
                        Close();
                        break;
                }
            });
            ShowDialog();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
        }
        private string radioContent;

        private void radio_Click(object sender, RoutedEventArgs e)
        {
            if(radio4.IsChecked == true)
            {
                other.Visibility = Visibility.Visible;
                radioContent = "4.其他:";
            }
            else
            {
                other.Text = "";
                other.Visibility = Visibility.Hidden;
                radioContent = Convert.ToString(((RadioButton)sender).Content);
            }
        }
    }
}