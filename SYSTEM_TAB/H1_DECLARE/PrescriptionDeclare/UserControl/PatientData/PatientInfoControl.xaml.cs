using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
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
    /// PatientInfoControl.xaml 的互動邏輯
    /// </summary>
    public partial class PatientInfoControl : System.Windows.Controls.UserControl,INotifyPropertyChanged
    {
        #region CheckDeclareStatus
        public static readonly DependencyProperty CheckDeclareStatusProperty =
            DependencyProperty.Register(
                "CheckDeclareStatus",
                typeof(ICommand),
                typeof(PatientInfoControl),
                new PropertyMetadata(null));
        public ICommand CheckDeclareStatus
        {
            get { return (ICommand)GetValue(CheckDeclareStatusProperty); }
            set { SetValue(CheckDeclareStatusProperty, value); }
        }
        #endregion       
        #region Prescription
        public static readonly DependencyProperty PrescriptionProperty =
            DependencyProperty.Register(
                "Prescription",
                typeof(Prescription),
                typeof(PatientInfoControl),
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
        #region SelectedPatientDetail
        public static readonly DependencyProperty SelectedPatientDetailProperty =
            DependencyProperty.Register(
                "SelectedPatientDetail",
                typeof(string),
                typeof(PatientInfoControl),
                new PropertyMetadata(null));
        public string SelectedPatientDetail
        {
            get { return (string)GetValue(SelectedPatientDetailProperty); }
            set
            {
                SetValue(SelectedPatientDetailProperty, value);
            }
        }
        #endregion
        #region CustomerEdited
        public static readonly DependencyProperty PatientEditedProperty =
            DependencyProperty.Register(
                "PatientEdited",
                typeof(bool),
                typeof(PatientInfoControl),
                new PropertyMetadata(null));
        public bool PatientEdited
        {
            get { return (bool)GetValue(PatientEditedProperty); }
            set
            {
                SetValue(PatientEditedProperty, value);
                OnPropertyChanged(nameof(PatientEdited));
            }
        }
        #endregion
        public PatientInfoControl()
        {
            InitializeComponent();
        }
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
