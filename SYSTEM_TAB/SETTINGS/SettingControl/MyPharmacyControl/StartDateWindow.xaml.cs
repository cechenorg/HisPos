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
using His_Pos.NewClass.Prescription.Treatment.Institution;

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
