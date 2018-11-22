using His_Pos.Class;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private ObservableCollection<CooperativeClinic> cooperativeClinicCollection;
        public ObservableCollection<CooperativeClinic> CooperativeClinicCollection
        {
            get => cooperativeClinicCollection;
            set
            {
                cooperativeClinicCollection = value;
                NotifyPropertyChanged("CooperativeClinicCollection");
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
            CooperativeClinicCollection = WebApi.GetXmlByMedicalNum(MainWindow.CurrentPharmacy.Id);

            Chronic chronic = new Chronic();
            chronic.hospital.Name = "新增新處方單";
            ChronicCollection.Add(chronic);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item;
            if (((Chronic)selectedItem).hospital.Name == "新增新處方單")
                Close();
            string decMasId = ((Chronic)selectedItem).DecMasId;
            PrescriptionDec2View.Instance.SetValueByDecMasId(decMasId);
            Close();
        }

        private void CooperativeClinicRow_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
            PrescriptionDec2View.Instance.SetValueByPrescription(((CooperativeClinic)selectedItem).Prescription);
            Close();
        }
    }
}
