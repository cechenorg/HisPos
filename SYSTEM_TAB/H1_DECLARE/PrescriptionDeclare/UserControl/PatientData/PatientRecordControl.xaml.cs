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
using His_Pos.NewClass.Person.Customer;
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
    public partial class PatientRecordControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        #region Prescription
        public static readonly DependencyProperty PatientProperty =
            DependencyProperty.Register(
                "Patient",
                typeof(Customer),
                typeof(PatientRecordControl),
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
        #endregion

        #region SelectedHistory
        public static readonly DependencyProperty SelectedRecordProperty =
            DependencyProperty.Register(
                "SelectedRecord",
                typeof(CustomerRecord),
                typeof(PatientRecordControl),
                new PropertyMetadata(null));
        public CustomerRecord SelectedRecord
        {
            get { return (CustomerRecord)GetValue(SelectedRecordProperty); }
            set
            {
                SetValue(SelectedRecordProperty, value);
                OnPropertyChanged(nameof(SelectedRecord));
            }
        }
        #endregion
        

        public PatientRecordControl()
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
         
        }

        private void Refresh(NotificationMessage msg)
        {
            if (!msg.Notification.Equals("PrescriptionEdited")) return;
            MainWindow.ServerConnection.OpenConnection();
            Patient.GetRecord();
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
