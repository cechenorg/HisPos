using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeSelectionWindow
{
    /// <summary>
    /// CooperativeSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeSelectionWindow : Window
    {
        public CooperativeSelectionWindow()
        {
            InitializeComponent();
            this.DataContext = new CooperativeSelectionViewModel();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseCooperativeSelection"))
                    Close();
            });
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
