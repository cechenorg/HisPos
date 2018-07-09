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
using His_Pos.AbstractClass;
using His_Pos.Class;
using His_Pos.Class.AdjustCase;
using His_Pos.Class.Copayment;
using His_Pos.Class.Division;
using His_Pos.Class.PaymentCategory;
using His_Pos.Class.Product;
using His_Pos.Class.TreatmentCase;
using His_Pos.Interface;
using His_Pos.ProductPurchase;

namespace His_Pos.H1_DECLARE.PrescriptionDec2
{
    /// <summary>
    /// PrescriptionDec2View.xaml 的互動邏輯
    /// </summary>
    public partial class PrescriptionDec2View : UserControl, INotifyPropertyChanged
    {
        private Prescription _prescription = new Prescription();

        public ObservableCollection<DeclareMedicine> DeclareMedicines { get; set; }
        public ObservableCollection<object> Medicines;
        public event PropertyChangedEventHandler PropertyChanged;

        public PrescriptionDec2View()
        {
            InitializeComponent();
            DataContext = this;
            GetPrescriptionData();
            //LoadPrescriptionData();
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

        private void LoadPrescriptionData()
        {
            LoadHospitalData();
            LoadTreatmentCases();
            LoadPaymentCategories();
            LoadCopayments();
            LoadAdjustCases();
        }

        private void GetPrescriptionData()
        {
            DeclareMedicines = new ObservableCollection<DeclareMedicine>();
            LoadingWindow loadingWindow = new LoadingWindow();
            loadingWindow.GetMedicinesData(this);
            loadingWindow.Show();
            loadingWindow.Topmost = true;
            LoadPrescriptionData();
            PrescriptionMedicines.ItemsSource = Prescription.Medicines;
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
            var copaymentDb = new CopaymentDb();
            copaymentDb.GetData();
            CopaymentCombo.ItemsSource = copaymentDb.Copayments;
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
            var adjustCaseDb = new AdjustCaseDb();
            adjustCaseDb.GetData();
            AdjustCaseCombo.ItemsSource = adjustCaseDb.AdjustCases;
        }

        private void Submit_ButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;

            if (selectedItem is IDeletable)
            {
                if (Prescription.Medicines.Contains(selectedItem))
                {
                    (selectedItem as IDeletable).Source = "/Images/DeleteDot.png";
                }

                PrescriptionMedicines.SelectedItem = selectedItem;
                return;
            }

            PrescriptionMedicines.SelectedIndex = Prescription.Medicines.Count;
        }

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow).Item;

            if (leaveItem is IDeletable)
            {
                (leaveItem as IDeletable).Source = string.Empty;
            }
        }

        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChanged();
            Prescription.Medicines.RemoveAt(PrescriptionMedicines.SelectedIndex);
        }

        private bool IsFirst = true;
        private bool IsChanged = false;
        private void SetChanged()
        {
            if (IsFirst == true) return;
            IsChanged = true;
        }
        private void SetIsChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        public AutoCompleteFilterPredicate<object> MedicineFilter
        {
            get
            {
                return (searchText, obj) =>
                    ((obj as DeclareMedicine).Id is null) ? false : (obj as DeclareMedicine).Id.ToLower().Contains(searchText.ToLower())
                                                            || (obj as DeclareMedicine).ChiName.ToLower().Contains(searchText.ToLower()) || (obj as DeclareMedicine).EngName.ToLower().Contains(searchText.ToLower());
            }
        }

        private void MedicineCodeAuto_Populating(object sender, PopulatingEventArgs e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;

            if (medicineCodeAuto is null) return;

            var result = DeclareMedicines.Where(x => x.Id.ToLower().Contains(medicineCodeAuto.Text.ToLower()) || x.ChiName.ToLower().Contains(medicineCodeAuto.Text.ToLower()) || x.EngName.ToLower().Contains(medicineCodeAuto.Text.ToLower())).Take(50).Select(x => x);
            Medicines = new ObservableCollection<object>(result.ToList());

            medicineCodeAuto.ItemsSource = Medicines;
            medicineCodeAuto.ItemFilter = MedicineFilter;
            medicineCodeAuto.PopulateComplete();
        }

        private void MedicineCodeAuto_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            var medicineCodeAuto = sender as AutoCompleteBox;
            SetChanged();
            if (medicineCodeAuto is null) return;
            if (medicineCodeAuto.SelectedItem is null)
            {
                if (medicineCodeAuto.Text != string.Empty && (medicineCodeAuto.ItemsSource as ObservableCollection<object>).Count != 0 && medicineCodeAuto.Text.Length >= 4)
                    medicineCodeAuto.SelectedItem = (medicineCodeAuto.ItemsSource as ObservableCollection<object>)[0];
                else
                    return;
            }

            Prescription.Medicines.Add(medicineCodeAuto.SelectedItem as DeclareMedicine);

            medicineCodeAuto.Text = "";
        }
    }
}