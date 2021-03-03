using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CommonHospitalsWindow
{
    /// <summary>
    /// CommonHospitalsWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CommonHospitalsWindow : Window
    {
        public CommonHospitalsWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCommonHospitalsWindow"))
                    Close();
            });
            DataContext = new CommonHospitalsViewModel();
            Closing += (sender, e) => Messenger.Default.Unregister(this);
            ShowDialog();
        }
    }
}