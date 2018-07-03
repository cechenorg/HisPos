using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.TreatmentCase;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl,INotifyPropertyChanged
    {
        private Prescription _prescription = new Prescription();
        public event PropertyChangedEventHandler PropertyChanged;

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            LoadHospitalData();
            LoadTreatmentCases();
            LoadPaymentCategories();
        }

        public Prescription Prescription
        {
            get
            {
                return _prescription;
            }
            set
            {
                _prescription = value;
                NotifyPropertyChanged("Prescription");
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /*
         *載入醫療院所資料
         */
        private void LoadHospitalData()
        {
            var hospitalDb = new HospitalDb();
            hospitalDb.GetData();
            ReleaseHospital.ItemsSource = hospitalDb.HospitalsCollection;
            LoadDivisionsData();
        }
        /*
         * 載入科別資料
         */
        private void LoadDivisionsData()
        {
            var divisionDb = new DivisionDb();
            divisionDb.GetData();
            DivisionCombo.ItemsSource = divisionDb.Divisions;
        }
        /*
         *載入給付類別
         */
        private void LoadPaymentCategories()
        {
            var paymentCategoryDb = new PaymentCategroyDb();
            paymentCategoryDb.GetData();
            PaymentCategoryCombo.ItemsSource = paymentCategoryDb.PaymentCategories;
        }
        /*
         *載入部分負擔
         */
        private void LoadCopayments()
        {
            CopaymentDb.GetData();
            foreach (var copayment in CopaymentDb.CopaymentList)
            {
                CopaymentCombo.Items.Add(copayment.Id + ". " + copayment.Name);
            }
        }
        /*
         *載入原處方案件類別
         */
        private void LoadTreatmentCases()
        {
            var treatmentCases = new TreatmentCaseDb();
            treatmentCases.GetData();
            TreatmentCaseCombo.ItemsSource = treatmentCases.TreatmentCases;
        }
        /*
         *載入原處方案件類別
         */
        private void LoadAdjustCases()
        {
            foreach (var adjustCase in AdjustCaseDb.AdjustCaseList)
            {
                AdjustCaseCombo.Items.Add(adjustCase.Id + ". " + adjustCase.Name);
            }
        }

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
