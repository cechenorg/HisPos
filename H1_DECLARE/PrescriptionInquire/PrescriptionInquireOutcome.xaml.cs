using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Declare;
using His_Pos.Class.Division;
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
        private bool isFirst = true;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        private ObservableCollection<TreatmentCase> treatmentCaseCollection;
        public ObservableCollection<TreatmentCase> TreatmentCaseCollection
        {
            get
            {
                return treatmentCaseCollection;
            }
            set
            {
                treatmentCaseCollection = value;
                NotifyPropertyChanged("TreatmentCaseCollection");
            }
        }
        private ObservableCollection<AdjustCase> adjustCaseCollection;
        public ObservableCollection<AdjustCase> AdjustCaseCollection
        {
            get
            {
                return adjustCaseCollection;
            }
            set
            {
                adjustCaseCollection = value;
                NotifyPropertyChanged("AdjustCaseCollection");
            }
        }
        private ObservableCollection<PaymentCategory> paymentCategoryCollection;
        public ObservableCollection<PaymentCategory> PaymentCategoryCollection
        {
            get
            {
                return paymentCategoryCollection;
            }
            set
            {
                paymentCategoryCollection = value;
                NotifyPropertyChanged("PaymentCategoryCollection");
            }
        }
        private ObservableCollection<Copayment> copaymentCollection;
        public ObservableCollection<Copayment> CopaymentCollection
        {
            get
            {
                return copaymentCollection;
            }
            set
            {
                copaymentCollection = value;
                NotifyPropertyChanged("CopaymentCollection");
            }
        }
        private ObservableCollection<Division> divisionCollection;
        public ObservableCollection<Division> DivisionCollection
        {
            get
            {
                return divisionCollection;
            }
            set
            {
                divisionCollection = value;
                NotifyPropertyChanged("DivisionCollection");
            }
        }
        private ObservableCollection<Hospital> hospitalCollection;
        public ObservableCollection<Hospital> HospitalCollection
        {
            get
            {
                return hospitalCollection;
            }
            set
            {
                hospitalCollection = value;
                NotifyPropertyChanged("HospitalCollection");
            }
        }
        private DeclareTrade declareTrade;
        
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
            InitData();
            InitDataChanged();
            isFirst = false;
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
        private void Text_TextChanged(object sender, EventArgs e)
        {
            DataChanged();
        }
        private void DataChanged()
        {
            if (isFirst) return;

            Changed.Content = "已修改";
            Changed.Foreground = Brushes.Red;

            ButtonImportXml.IsEnabled = true;
        }
        
        private void InitDataChanged()
        {
            Changed.Content = "未修改";
            Changed.Foreground = Brushes.Black;

            ButtonImportXml.IsEnabled = false;
        }
        private void InitData() {
            HospitalCollection = HospitalDb.GetData();
            ReleasePalace.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.FullName;

            DivisionCollection = DivisionDb.GetData();
            Division.ItemsSource = DivisionCollection;
            Division.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.Hospital.Division.FullName;

            CopaymentCollection = CopaymentDb.GetData();
            CopaymentCode.ItemsSource = CopaymentCollection;
            CopaymentCode.Text = InquiredPrescription.Prescription.Treatment.Copayment.FullName;

            PaymentCategoryCollection = PaymentCategroyDb.GetData();
            PaymentCategory.ItemsSource = PaymentCategoryCollection;
            PaymentCategory.Text = InquiredPrescription.Prescription.Treatment.PaymentCategory.FullName;

            AdjustCaseCollection = AdjustCaseDb.GetData();
            AdjustCase.ItemsSource = AdjustCaseCollection;
            AdjustCase.Text = InquiredPrescription.Prescription.Treatment.AdjustCase.FullName;

            TreatmentCaseCollection = TreatmentCaseDb.GetData();
            TreatmentCase.ItemsSource = TreatmentCaseCollection;
            TreatmentCase.Text = InquiredPrescription.Prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
        }

        private void ReleasePalace_Populating(object sender, PopulatingEventArgs e)
        {
            ObservableCollection<Hospital> tempCollection = new ObservableCollection<Hospital>(HospitalCollection.Where(x => x.Id.Contains(ReleasePalace.Text)).Take(50).ToList());
            ReleasePalace.ItemsSource = tempCollection;
            ReleasePalace.PopulateComplete();
        }
    }
}