using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.ChromeTabViewModel;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.CooperativeSelectionWindow
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
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
