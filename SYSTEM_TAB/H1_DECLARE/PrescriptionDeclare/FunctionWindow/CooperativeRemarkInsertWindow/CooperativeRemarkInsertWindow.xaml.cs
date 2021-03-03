using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow
{
    /// <summary>
    /// CooperativeRemarkInsertWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeRemarkInsertWindow : Window
    {
        public CooperativeRemarkInsertWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CooperativeRemarkInsert"))
                    Close();
            });
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            ShowDialog();
        }
    }
}