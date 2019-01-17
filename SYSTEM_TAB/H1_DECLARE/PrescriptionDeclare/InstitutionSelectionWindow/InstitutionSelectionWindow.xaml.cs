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

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.InstitutionSelectionWindow
{
    /// <summary>
    /// InstitutionSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InstitutionSelectionWindow : Window
    {
        public InstitutionSelectionWindow(string searchText)
        {
            InitializeComponent();
            this.DataContext = new InstitutionSelectionViewModel(searchText);
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseInstitutionSelection"))
                    Close();
            });
            this.Unloaded += (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
