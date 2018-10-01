using His_Pos.Class;
using His_Pos.Class.Declare;
using His_Pos.Class.Product;
using His_Pos.Service;
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
            PrescriptionDec2View.Instance.SetValueByDecMasId(decMasId);

            Close();
        }
    }
}
