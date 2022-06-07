using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Prescription.Treatment.Institution;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.SETTINGS.SettingControl.MyPharmacyControl
{
    /// <summary>
    /// StartDateWindow.xaml 的互動邏輯
    /// </summary>
    public partial class StartDateWindow : Window
    {
        public StartDateWindow()
        {
            InitializeComponent();
        }

        public StartDateWindow(Pharmacy myPharmacy)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseStartDateViewModel"))
                    Close();
            });
            this.DataContext = new StartDateViewModel(myPharmacy);
        }
    }
}