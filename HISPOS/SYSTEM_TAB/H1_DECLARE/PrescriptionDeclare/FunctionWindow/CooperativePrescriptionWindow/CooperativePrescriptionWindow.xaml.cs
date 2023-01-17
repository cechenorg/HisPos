using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativePrescriptionWindow
{
    /// <summary>
    /// CooperativePrescriptionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativePrescriptionWindow : Window
    {
        public CooperativePrescriptionWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCooperativePrescriptionWindow"))
                    Close();
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            ShowDialog();
        }
    }
}