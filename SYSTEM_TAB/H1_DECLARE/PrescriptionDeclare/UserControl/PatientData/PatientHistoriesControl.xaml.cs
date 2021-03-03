using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription.Service;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientHistoriesControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientHistoriesControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        #region Prescription

        public static readonly DependencyProperty PatientProperty =
            DependencyProperty.Register(
                "Patient",
                typeof(Customer),
                typeof(PatientHistoriesControl),
                new PropertyMetadata(null));

        public Customer Patient
        {
            get { return (Customer)GetValue(PatientProperty); }
            set
            {
                SetValue(PatientProperty, value);
                OnPropertyChanged(nameof(Patient));
            }
        }

        #endregion Prescription

        #region SelectedHistory

        public static readonly DependencyProperty SelectedHistoryProperty =
            DependencyProperty.Register(
                "SelectedHistory",
                typeof(CustomerHistory),
                typeof(PatientHistoriesControl),
                new PropertyMetadata(null));

        public CustomerHistory SelectedHistory
        {
            get { return (CustomerHistory)GetValue(SelectedHistoryProperty); }
            set
            {
                SetValue(SelectedHistoryProperty, value);
                OnPropertyChanged(nameof(SelectedHistory));
            }
        }

        #endregion SelectedHistory

        public PatientHistoriesControl()
        {
            InitializeComponent();
        }

        private void ShowPrescriptionEditWindow(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row?.Item is null) return;
            if (!(row.Item is CustomerHistory)) return;
            EditPrescription();
        }

        private void EditPrescription()
        {
            if (SelectedHistory is null) return;
            Messenger.Default.Register<NotificationMessage>(this, Refresh);
            PrescriptionService.ShowPrescriptionEditWindow(SelectedHistory.SourceId);
            Messenger.Default.Unregister<NotificationMessage>(this, Refresh);
        }

        private void Refresh(NotificationMessage msg)
        {
            if (!msg.Notification.Equals("PrescriptionEdited")) return;
            MainWindow.ServerConnection.OpenConnection();
            Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}