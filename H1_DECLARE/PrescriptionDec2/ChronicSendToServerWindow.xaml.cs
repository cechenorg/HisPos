using His_Pos.Class;
using His_Pos.Class.Product;
using His_Pos.Interface;
using His_Pos.ProductPurchase;
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
    /// ChronicSendToServerWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ChronicSendToServerWindow : Window , INotifyPropertyChanged {

        public class PrescriptionSendData:INotifyPropertyChanged {
            public PrescriptionSendData(DeclareMedicine declareMedicine) {
                MedId = declareMedicine.Id;
                MedName = declareMedicine.Name;
                Stock = ChronicDb.GetResidualAmountById(MedId);
                TreatAmount = declareMedicine.Amount.ToString();
                SendAmount = "0";
            }

            public string MedId { get; set; }
            public string MedName { get; set; }
            public string Stock { get; set; }
            public string TreatAmount { get; set; }
            public string SendAmount { get; set; }
            private string source;

            public string Source
            {
                get { return source; }
                set
                {
                    source = value;
                    NotifyPropertyChanged("Source");
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      
        private ObservableCollection<PrescriptionSendData> prescription = new ObservableCollection<PrescriptionSendData>();
        public ObservableCollection<PrescriptionSendData> Prescription {
            get => prescription;
            set
            {
                prescription = value;
                NotifyPropertyChanged("Prescription");
            }
        }
        public static ChronicSendToServerWindow Instance;
        public ChronicSendToServerWindow(ObservableCollection<DeclareMedicine> medicines) {
            InitializeComponent();
            foreach (DeclareMedicine row in medicines) {
                Prescription.Add(new PrescriptionSendData(row));
            } 
            DataContext = this;
            Instance = this;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ButtonSubmmit_Click(object sender, RoutedEventArgs e) {
            foreach (PrescriptionSendData prescriptionSendData in Prescription) {
                if (prescriptionSendData.SendAmount == "0") {
                    MessageWindow messageWindow = new MessageWindow("傳輸量不可為0",MessageType.ERROR, true);
                    messageWindow.ShowDialog();
                    return;
                }
            }
            PrescriptionDec2View.Instance.IsSend = true;
            PrescriptionDec2View.Instance.PrescriptionSendData = Prescription;
            Close();
        }
        private void DeleteDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Prescription.RemoveAt(MedicinesList.SelectedIndex);
        }

        private void DataGridRow_MouseEnter(object sender, MouseEventArgs e)
        {
            var selectedItem = (sender as DataGridRow).Item;
                if (Prescription.Contains(selectedItem))
                    (selectedItem as PrescriptionSendData).Source = "/Images/DeleteDot.png"; 
                MedicinesList.SelectedItem = selectedItem; 
        } 

        private void DataGridRow_MouseLeave(object sender, MouseEventArgs e)
        {
            var leaveItem = (sender as DataGridRow)?.Item;
             (leaveItem as PrescriptionSendData).Source = string.Empty;
        }
    }
}
