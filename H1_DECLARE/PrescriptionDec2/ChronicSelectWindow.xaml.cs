using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Product;
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
using System.Windows.Shapes;

namespace His_Pos.H1_DECLARE.PrescriptionDec2 {
    /// <summary>
    /// ChronicSelectWindow.xaml 的互動邏輯
    /// </summary> 
    public partial class ChronicSelectWindow : Window, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged; 
        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<Chronic> chronicCollection;
        public ObservableCollection<Chronic> ChronicCollection
        {
            get => chronicCollection;
            set
            {
                chronicCollection = value;
                NotifyPropertyChanged("ChronicCollection");
            }
        }
        private string CusId;
        public Chronic selectChronic;
        public ChronicSelectWindow(string cusId) {
            InitializeComponent();
            CusId = cusId;
            InitData();
            DataContext = this;
        }
        private void InitData() {
            ChronicCollection = ChronicDb.GetChronicDeclareById(CusId);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item;
            string decMasId = ((Chronic)selectedItem).DecMasId;
            PrescriptionDec2View.Instance.CurrentDecMasId = decMasId;

            Prescription prescription = PrescriptionDB.GetDeclareDataById(decMasId).Prescription;
            PrescriptionDec2View.Instance.CurrentPrescription = prescription; 
            PrescriptionDec2View.Instance.DivisionCombo.Text = prescription.Treatment.MedicalInfo.Hospital.Division.FullName;
            PrescriptionDec2View.Instance.AdjustCaseCombo.Text = prescription.Treatment.AdjustCase.FullName;
            PrescriptionDec2View.Instance.TreatmentCaseCombo.Text = prescription.Treatment.MedicalInfo.TreatmentCase.FullName;
            PrescriptionDec2View.Instance.PaymentCategoryCombo.Text = prescription.Treatment.PaymentCategory.FullName;
            PrescriptionDec2View.Instance.CopaymentCombo.Text = prescription.Treatment.Copayment.FullName;
            PrescriptionDec2View.Instance.SpecialCode.Text = prescription.Treatment.MedicalInfo.SpecialCode.Id; 
            PrescriptionDec2View.Instance.DatePickerPrecription.Text = prescription.Treatment.TreatmentDate.ToString("yyyy/MM/dd");
            PrescriptionDec2View.Instance.DatePickerTreatment.Text = prescription.Treatment.AdjustDate.ToString("yyyy/MM/dd");

            PrescriptionDec2View.Instance.CurrentPrescription.Medicines = MedicineDb.GetDeclareMedicineByMasId(decMasId);
            PrescriptionDec2View.Instance.PrescriptionMedicines.ItemsSource = PrescriptionDec2View.Instance.CurrentPrescription.Medicines;

            Close();
        }
    }
}
