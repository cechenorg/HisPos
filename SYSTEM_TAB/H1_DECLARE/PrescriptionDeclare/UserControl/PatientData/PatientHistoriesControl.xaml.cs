using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using His_Pos.NewClass.Person.Customer.CustomerHistory;
using His_Pos.NewClass.Prescription;
using His_Pos.NewClass.Prescription.Service;
using His_Pos.NewClass.Product.CustomerHistoryProduct;
using JetBrains.Annotations;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDeclare.UserControl.PatientData
{
    /// <summary>
    /// PatientHistoriesControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientHistoriesControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        #region Prescription
        public static readonly DependencyProperty PrescriptionProperty =
            DependencyProperty.Register(
                "Prescription",
                typeof(Prescription),
                typeof(PatientHistoriesControl),
                new PropertyMetadata(null));
        public Prescription Prescription
        {
            get { return (Prescription)GetValue(PrescriptionProperty); }
            set
            {
                SetValue(PrescriptionProperty, value);
                OnPropertyChanged(nameof(Prescription));
            }
        }
        #endregion
        #region CopyComplete
        public static readonly DependencyProperty CopyCompleteProperty =
            DependencyProperty.Register(
                "CopyComplete",
                typeof(ICommand),
                typeof(PatientHistoriesControl),
                new PropertyMetadata(null));
        public ICommand CopyComplete
        {
            get { return (ICommand)GetValue(CopyCompleteProperty); }
            set { SetValue(CopyCompleteProperty, value); }
        }
        #endregion
        private CustomerHistory selectedHistory;
        public CustomerHistory SelectedHistory
        {
            get => selectedHistory;
            set
            {
                if (value != null)
                {
                    MainWindow.ServerConnection.OpenConnection();
                    value.Products = new CustomerHistoryProducts();
                    value.Products.GetCustomerHistoryProducts(value.SourceId, value.Type);
                    MainWindow.ServerConnection.CloseConnection();
                }
                selectedHistory = value;
                OnPropertyChanged(nameof(SelectedHistory));
            }
        }

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
            Prescription.Patient.GetHistories();
            MainWindow.ServerConnection.CloseConnection();
        }

        private void CopyPrescription(object sender, RoutedEventArgs e)
        {
            MainWindow.ServerConnection.OpenConnection();
            var prescription = SelectedHistory.GetPrescriptionRefactoringByID();
            MainWindow.ServerConnection.CloseConnection();
            prescription.TreatDate = null;
            prescription.AdjustDate = null;
            prescription.TempMedicalNumber = null;
            prescription.Patient = Prescription.Patient;
            prescription.PrescriptionStatus.Init();
            prescription.Reset();
            Prescription = prescription;
            Prescription.ID = 0;
            CopyComplete.Execute(null);
        }




        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
