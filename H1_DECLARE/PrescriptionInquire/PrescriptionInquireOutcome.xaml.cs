using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.TreatmentCase;
using His_Pos.Properties;
using His_Pos.Service;

namespace His_Pos.PrescriptionInquire
{
    /// <summary>
    /// PrescriptionInquireOutcome.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionInquireOutcome : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private DeclareTrade declareTrade;
        //GetDeclarTradeByMasId
        public DeclareTrade DeclareTrade
        {
            get
            {
                return declareTrade;
            }
            set
            {
                declareTrade = value;
                NotifyPropertyChanged("DeclareTrade");
            }
        }
        private static DeclareData inquiredPrescription;
        public DeclareData InquiredPrescription
        {
            get
            {
                return inquiredPrescription;
            }
            set
            {
                inquiredPrescription = value;
                NotifyPropertyChanged("InquiredPrescription");
            }
        }
        public  ObservableCollection<DeclareDetail> medicineList = new ObservableCollection<DeclareDetail>();

        public PrescriptionInquireOutcome(DeclareData inquired)
        {
            InitializeComponent();
            DataContext = this;
            DeclareTrade = DeclareTradeDb.GetDeclarTradeByMasId(inquired.DecMasId);
            InquiredPrescription = inquired;
            SetPatientData();
        }

        private void SetPatientData()
        {
            var patient = InquiredPrescription.Prescription.Customer;
            var patientGenderIcon = new BitmapImage(new Uri(@"..\..\Images\Male.png", UriKind.Relative));
            var patientIdIcon = new BitmapImage(new Uri(@"..\..\Images\ID_Card.png", UriKind.Relative));
            var patientBirthIcon = new BitmapImage(new Uri(@"..\..\Images\birthday.png", UriKind.Relative));
            //var patientEmergentPhoneIcon = new BitmapImage(new Uri(@"..\..\Images\Phone.png", UriKind.Relative));
            PatientName.SetIconSource(patientGenderIcon);
            PatientId.SetIconSource(patientIdIcon);
            PatientBirthday.SetIconSource(patientBirthIcon);
            //PatientTel.SetIconSource(patientEmergentPhoneIcon);
            //PatientTel.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.ContactInfo.Tel);
            PatientBirthday.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.Birthday);
            PatientId.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.IcNumber);
            PatientName.SetIconLabel(200, 50, InquiredPrescription.Prescription.Customer.Name);
        }

        private void DataGridRow_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            PrescriptionSet.SelectedItem = (sender as DataGridRow).Item;
        }
    }
}