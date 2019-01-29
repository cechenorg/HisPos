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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.CooperativeRemarkInsertWindow {
    /// <summary>
    /// CooperativeRemarkInsertWindow.xaml 的互動邏輯
    /// </summary>
    public partial class CooperativeRemarkInsertWindow : Window {
        public CooperativeRemarkInsertWindow() {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CooperativeRemarkInsert"))
                    Close();
            });
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
            ShowDialog();
        }
    }
}
