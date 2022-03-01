using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.DeclareFileManage.AdjustPharmacistSetting
{
    /// <summary>
    /// AdjustPharmacistWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AdjustPharmacistWindow : Window
    {
        private AdjustPharmacistViewModel adjustPharmacistViewModel;

        public AdjustPharmacistWindow(DateTime declare)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseAdjustPharmacistWindow"))
                    Close();
            });
            adjustPharmacistViewModel = new AdjustPharmacistViewModel(declare);
            DataContext = adjustPharmacistViewModel;
            ShowDialog();
            UpdateLayout();
        }

        private void MonthViewCalendar_MouseLeave(object sender, MouseEventArgs e)
        {
            Keyboard.ClearFocus();
        }
    }
}