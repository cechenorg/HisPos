using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow.WareHouseSelectWindow
{
    /// <summary>
    /// WareHouseSelectWindow.xaml 的互動邏輯
    /// </summary>
    public partial class WareHouseSelectWindow : Window
    {
        public WareHouseSelectWindow(DateTime sDate, DateTime eDate)
        {
            InitializeComponent();
            WareHouseSelectViewModel wareHouseSelectViewModel = new WareHouseSelectViewModel(sDate, eDate);
            DataContext = wareHouseSelectViewModel;
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification == "CloseWareHouseSelectWindow")
                    Close();
            });
            ShowDialog();
        }
    }
}