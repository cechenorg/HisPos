using GalaSoft.MvvmLight.Messaging;
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

namespace His_Pos.SYSTEM_TAB.H7_ACCOUNTANCY_REPORT.ControlMedicineDeclare.ControlMedicineEditWindow.WareHouseSelectWindow
{
    /// <summary>
    /// WareHouseSelectWindow.xaml 的互動邏輯
    /// </summary>
    public partial class WareHouseSelectWindow : Window
    {
        public WareHouseSelectWindow(DateTime sDate,DateTime eDate)
        {
            InitializeComponent();
            WareHouseSelectViewModel wareHouseSelectViewModel = new WareHouseSelectViewModel(sDate,eDate);
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
