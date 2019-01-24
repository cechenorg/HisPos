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
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.FunctionWindow.ErrorUploadWindow
{
    /// <summary>
    /// ErrorUploadWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ErrorUploadWindow : Window
    {
        public ErrorUploadWindow(bool isGetMedicalNumber)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                switch (notificationMessage.Notification)
                {
                    case "CloseErrorUploadWindow":
                        Close();
                        break;
                }
            });
            this.DataContext = new ErrorUploadWindowViewModel(isGetMedicalNumber);
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
