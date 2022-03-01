using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditInputWindow
{
    /// <summary>
    /// ControlMedicineEditInputWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ControlMedicineEditInputWindow : Window
    {
        public ControlMedicineEditInputWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseControlMedicineEditInputWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}