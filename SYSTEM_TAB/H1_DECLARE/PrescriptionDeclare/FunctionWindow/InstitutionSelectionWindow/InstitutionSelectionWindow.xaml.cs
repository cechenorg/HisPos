using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow
{
    /// <summary>
    /// InstitutionSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InstitutionSelectionWindow : Window
    {
        public InstitutionSelectionWindow(string searchText,ViewModelEnum vm)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseInstitutionSelection"))
                    Close();
            });
            this.DataContext = new InstitutionSelectionViewModel(searchText, vm);
            SearchStringTextBox.Focus();
            this.Closing+= (sender, e) => Messenger.Default.Unregister(this);
        }
    }
}
