using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.FunctionWindow.InstitutionSelectionWindow
{
    /// <summary>
    /// InstitutionSelectionWindow.xaml 的互動邏輯
    /// </summary>
    public partial class InstitutionSelectionWindow : Window
    {
        private InstitutionSelectionViewModel institutionSelectionViewModel { get; set; }

        public InstitutionSelectionWindow(string searchText)
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, (notificationMessage) =>
            {
                if (notificationMessage.Notification.Equals("CloseInstitutionSelection"))
                    Close();
            });
            institutionSelectionViewModel = new InstitutionSelectionViewModel(searchText);
            this.DataContext = institutionSelectionViewModel;
            SearchStringTextBox.Focus();
            this.Closing += (sender, e) => Messenger.Default.Unregister(this);
            if (institutionSelectionViewModel.ShowDialog)
                ShowDialog();
        }
    }
}