using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using His_Pos.Class;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.PrescriptionDec2 {
    /// <summary>
    /// ChronicSelectWindow.xaml 的互動邏輯
    /// </summary> 
    public partial class ChronicSelectWindow : Window, INotifyPropertyChanged {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

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
        private string CusIcNum;
        public Chronic selectChronic;
        public ChronicSelectWindow(string cusId,string cusIcNum) {
            InitializeComponent();
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            CusId = cusId;
            CusIcNum = cusIcNum;
            InitData();
            DataContext = this;
        }
        private void InitData() {
           /// ChronicCollection = ChronicDb.GetChronicDeclareById(CusId); 
           /// CooperativeClinicCollection = WebApi.GetXmlByMedicalNum(MainWindow.CurrentPharmacy.Id, CusIcNum);
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var selectedItem = (sender as DataGridRow).Item; 
            string decMasId = ((Chronic)selectedItem).DecMasId;
            PrescriptionDec2View.Instance.SetValueByDecMasId(decMasId);
            Close();
        }

        private void CooperativeClinicRow_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
            PrescriptionDec2View.Instance.SetValueByPrescription(((CooperativeClinic)selectedItem));
            
            Close();
        }

        private void ButtonNewPrescription_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
